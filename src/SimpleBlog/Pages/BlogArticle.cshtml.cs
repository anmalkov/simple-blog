using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class ArticleModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;

        public string Id { get; private set; }
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Created { get; private set; }
        public List<string> Tags { get; private set; }

        public ArticleModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
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
        }
    }
}
