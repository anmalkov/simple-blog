using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
    public class AdminArticleModel : PageModel
    {
        private readonly IArticlesService _articlesService;

        [BindProperty]
        [Required]
        [Display(Name = "URL name")]
        public string ArticleId { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Tags (comma separated)")]
        public string Tags { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Published")]
        public bool Published { get; set; }

        public AdminArticleModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        public async Task OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Published = true;
                return;
            }

            var article = await _articlesService.GetAsync(id);

            MapFromArticle(article);
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var article = MapToArticle();

            if (string.IsNullOrEmpty(id))
            {
                await _articlesService.CreateAsync(article);
            } else
            {
                await _articlesService.UpdateAsync(article);
            }

            return RedirectToPage("/admin");
        }

        private Article MapToArticle()
        {
            return new Article
            {
                Id = ArticleId,
                Title = Title,
                Body = Body,
                Tags = new List<string>(Tags.Split(',')),
                Published = Published
            };
        }
        private void MapFromArticle(Article article)
        {
            ArticleId = article.Id;
            Title = article.Title;
            Body = article.Body;
            Tags = string.Join(", ", article.Tags);
            Published = article.Published;
        }
    }
}
