using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Services;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;

        private Dictionary<string, ArticleInfo> _articleInfos;

        public List<ArticleViewModel> Articles { get; set; }

        public AdminModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        public async Task OnGet()
        {
            var articleInfos = (await _articlesService.GetAllArticleInfosAsync()).ToDictionary(a => a.ArticleId, a => a);
            Articles = (await _articlesService.GetAllAsync()).Select(a => MapToViewModel(a, articleInfos.ContainsKey(a.Id) ? articleInfos[a.Id] : null)).ToList();
        }

        public async Task<IActionResult> OnPostDeleteArticleAsync(string id)
        {
            await _articlesService.DeteleAsync(id);
            return RedirectToPage("/admin");
        }

        private ArticleViewModel MapToViewModel(Article article, ArticleInfo articleInfo)
        {
            return new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Created = article.Created,
                Published = article.Published,
                ViewsCount = articleInfo != null ? articleInfo.ViewsCount : 0,
                CommentsCount = articleInfo != null ? articleInfo.CommentsCount : 0
            };
        }
    }
}
