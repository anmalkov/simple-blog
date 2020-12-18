using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminPageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IPagesService _pagesService;
        private readonly IImagesRepository _imagesRepository;

        [BindProperty]
        [Required]
        [Display(Name = "URL name")]
        public string PageId { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }

        public List<string> Images { get; set; }

        [BindProperty]
        public List<IFormFile> UploadImages { get; set; }

        public AdminPageModel(IPagesService pagesService, IImagesRepository imagesRepository)
        {
            _pagesService = pagesService;
            _imagesRepository = imagesRepository;
        }

        public async Task OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Images = new List<string>();
                return;
            }

            var page = await _pagesService.GetAsync(id);
            MapFromPage(page);

            Images = (await _imagesRepository.GetAllAsync(id)).Select(i => Path.GetFileName(i)).ToList();
        }

        public async Task<IActionResult> OnPostImageAsync(string id)
        {
            foreach (var image in UploadImages)
            {
                await _imagesRepository.UploadAsync(id, image);
            }
            return RedirectToPage("/adminpage", new { id });
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(string id, string fileName)
        {
            await _imagesRepository.DeleteAsync(id, fileName);
            return RedirectToPage("/adminpage", new { id });
        }

        public async Task<IActionResult> OnPostDataAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var page = MapToPage();

            if (string.IsNullOrEmpty(id))
            {
                await _pagesService.CreateAsync(page);
            }
            else
            {
                await _pagesService.UpdateAsync(page);
            }

            return RedirectToPage("/adminpages");
        }

        private Page MapToPage()
        {
            return new Page
            {
                Id = PageId,
                Title = Title,
                Body = Body
            };
        }
        private void MapFromPage(Page page)
        {
            PageId = page.Id;
            Title = page.Title;
            Body = page.Body;
        }
    }
}
