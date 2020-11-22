using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using SimpleBlog.Configuration;
using SimpleBlog.Model;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class BlogModel : PageModel
    {
        private readonly IArticlesService _articlesService;
        private readonly int _pageSize;

        public List<Article> Articles { get; set; }
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public bool NextPageAvailable => TotalNumberOfPages > PageNumber;
        public bool PreviousPageAvailable => PageNumber > 1;

        public BlogModel(IArticlesService articlesService, IOptions<BlogConfiguration> configuration)
        {
            _articlesService = articlesService;
            _pageSize = configuration.Value.BlogPageSize;
        }

        public async Task OnGet(int? page)
        {
            if (!page.HasValue)
            {
                page = 1;
            }

            PageNumber = page.Value;

            TotalNumberOfPages = await _articlesService.GetTotalNumberOfPagesAsync(_pageSize);

            Articles = await _articlesService.GetAllAsync(PageNumber, _pageSize);
        }
    }
}
