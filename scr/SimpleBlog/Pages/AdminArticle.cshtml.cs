using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Id { get; set; }

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

        public void OnGet()
        {
            Published = true;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var article = MapToArticle();
            await _articlesService.CreateAsync(article);

            return RedirectToPage("/admin");
        }

        private Article MapToArticle()
        {
            return new Article
            {
                Id = Id,
                Title = Title,
                Body = Body,
                Tags = new List<string>(Tags.Split(','))
            };
        }
    }
}
