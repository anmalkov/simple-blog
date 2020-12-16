using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Model;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminPageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IPagesService _pagesService;

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

        public AdminPageModel(IPagesService pagesService)
        {
            _pagesService = pagesService;
        }

        public async Task OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            var page = await _pagesService.GetAsync(id);

            MapFromPage(page);
        }

        public async Task<IActionResult> OnPost(string id)
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
