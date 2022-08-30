using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class ArticleModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;
        private readonly ISiteConfigurationRepository _configRepository;

        public string Id { get; private set; }
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Created { get; private set; }
        public List<string> Tags { get; private set; }
        public bool ShowRecommendedArticles { get; private set; }
        public List<Article> RecommendedArticles { get; set; }

        public ArticleModel(IArticlesService articlesService, ISiteConfigurationRepository configRepository)
        {
            _articlesService = articlesService;
            _configRepository = configRepository;
        }

        public async Task OnGetAsync(string id)
        {
            var article = await _articlesService.GetAsync(id);
            if (article == null || (!article.Published && !User.Identity.IsAuthenticated))
            {
                Title = "404 Not Found";
                HtmlBody = "Unfortunately this blog post was not found.";
                Created = DateTime.Now;
                return;
            }
            Id = article.Id;
            Title = article.Title;
            HtmlBody = article.HtmlBody;
            Created = article.Created;
            Tags = new List<string>(article.Tags);

            await _articlesService.IncrementViewsCounterAsync(article.Id);

            var config = await _configRepository.GetAsync();
            ShowRecommendedArticles = config.RecommendedArticlesCount > 0;
            if (ShowRecommendedArticles)
            {
                RecommendedArticles = await _articlesService.GetRecommendedByTagsAsync(article.Tags, config.RecommendedArticlesCount);
            }
        }
    }
}
