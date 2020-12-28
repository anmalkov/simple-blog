using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class IndexModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IPagesService _pagesService;
        private readonly IArticlesService _articlesService;
        private readonly ISiteConfigurationRepository _configRepository;
        private readonly ILogger<IndexModel> _logger;

        public string Id { get; private set; }
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }
        public List<Article> LatestArticles { get; private set; }
        public List<Article> PopularArticles { get; private set; }


        public IndexModel(ILogger<IndexModel> logger, IPagesService pagesService, IArticlesService articlesService, ISiteConfigurationRepository configRepository)
        {
            _logger = logger;
            _pagesService = pagesService;
            _articlesService = articlesService;
            _configRepository = configRepository;
        }


        public async Task OnGetAsync()
        {
            var page = await _pagesService.GetIndexAsync();
            if (page == null)
            {
                Title = "404 Not Found";
                HtmlBody = "Unfortunately this page was not found.";
                return;
            }
            Id = page.Id;
            Title = page.Title;
            HtmlBody = page.HtmlBody;

            var latestBlogPostsCount = (await _configRepository.GetAsync()).LatestBlogPostsCount;
            LatestArticles = await _articlesService.GetLatestAsync(latestBlogPostsCount);
            PopularArticles = await _articlesService.GetMostPopularAsync(latestBlogPostsCount);
        }
    }
}
