using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminArticleModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;
        private readonly IImagesRepository _imagesRepository;

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

        public List<string> Images { get; set; }

        [BindProperty]
        public List<IFormFile> UploadImages { get; set; }

        public AdminArticleModel(IArticlesService articlesService, IImagesRepository imagesRepository)
        {
            _articlesService = articlesService;
            _imagesRepository = imagesRepository;
        }

        public async Task OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Published = true;
                Images = new List<string>();
                return;
            }

            var article = await _articlesService.GetAsync(id);

            Images = (await _imagesRepository.GetAllAsync(id)).Select(i => Path.GetFileName(i)).ToList();

            MapFromArticle(article);
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(string id, string fileName)
        {
            await _imagesRepository.DeleteAsync(id, fileName);
            return RedirectToPage("/adminarticle", new { id });
        }

        public async Task<IActionResult> OnPostImageAsync(string id)
        {
            foreach (var image in UploadImages)
            {
                await _imagesRepository.UploadAsync(id, image);
            }
            return RedirectToPage("/adminarticle", new { id });
        }

        public async Task<IActionResult> OnPostDataAsync(string id)
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
                Tags = new List<string>(Tags.Split(',').Select(t => t.Trim())),
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
