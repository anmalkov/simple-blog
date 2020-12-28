using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class BlogTagsModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;

        public Dictionary<string, List<Article>> Tags { get; set; }

        public BlogTagsModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        public async Task OnGetAsync()
        {
            var tags = await _articlesService.GetAllTagsAsync();
            Tags = new Dictionary<string, List<Article>>();
            foreach (var tag in tags)
            {
                var articles = await _articlesService.GetAllAsync(tag);
                Tags.Add(tag, articles);
            }
        }
    }
}
