using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;
        public List<Article> Articles { get; set; }

        public AdminModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        public async Task OnGet()
        {
            Articles = await _articlesService.GetAllAsync();
        }
    }
}
