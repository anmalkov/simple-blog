using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class BlogModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;
        private readonly int _pageSize;

        public List<Article> Articles { get; set; }
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public bool NextPageAvailable => TotalNumberOfPages > PageNumber;
        public bool PreviousPageAvailable => PageNumber > 1;

        public BlogModel(IArticlesService articlesService, ISiteConfigurationRepository configRepository)
        {
            _articlesService = articlesService;
            _pageSize = configRepository.GetAsync().Result.BlogPostsPageSize;
        }

        public async Task OnGet(int? pageNumber)
        {
            if (!pageNumber.HasValue)
            {
                pageNumber = 1;
            }

            PageNumber = pageNumber.Value;

            TotalNumberOfPages = await _articlesService.GetTotalNumberOfPagesAsync(_pageSize);

            Articles = await _articlesService.GetAllAsync(PageNumber, _pageSize);
        }
    }
}
