using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class BlogTagModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;

        public string Tag { get; set; }
        public List<Article> Articles { get; set; }

        public BlogTagModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }
        
        public async Task OnGet(string tag)
        {
            Tag = tag;
            Articles = await _articlesService.GetAllAsync(tag);
        }
    }
}
