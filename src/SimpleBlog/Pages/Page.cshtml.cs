using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class PageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IPagesService _pagesService;

        public string Id { get; private set; }
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }

        public PageModel(IPagesService pagesService)
        {
            _pagesService = pagesService;
        }

        public async Task OnGetAsync(string id)
        {
            var page = await _pagesService.GetAsync(id);
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
