using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class IndexModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IPagesService _pagesService;
        private readonly ILogger<IndexModel> _logger;

        public string Id { get; private set; }
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }
        
        public IndexModel(ILogger<IndexModel> logger, IPagesService pagesService)
        {
            _logger = logger;
            _pagesService = pagesService;
        }

        public async Task OnGetAsync(string id)
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
        }
    }
}
