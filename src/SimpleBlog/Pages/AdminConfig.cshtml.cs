using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Repositories;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminConfigModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly ISiteConfigurationRepository _configRepository;

        [BindProperty]
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "How many posts on one page")]
        public int BlogPostsPageSize { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "How many latest and popular posts on index page")]
        public int LatestBlogPostsCount { get; set; }

        public List<MenuItem> MenuItems { get; set; }

        public AdminConfigModel(ISiteConfigurationRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task OnGet()
        {
            var config = await _configRepository.GetAsync();
            MapFromSiteConfiguration(config);

            MenuItems = await _configRepository.GetAllMenuItemsAsync();
        }

        public async Task<IActionResult> OnPostDeleteMenuItemAsync(string title)
        {
            await _configRepository.DeleteAsync(title);
            return RedirectToPage("/adminconfig");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var config = MapToSiteConfiguration();

            await _configRepository.UpdateAsync(config);

            return RedirectToPage("/adminconfig");
        }

        private SiteConfiguration MapToSiteConfiguration()
        {
            return new SiteConfiguration
            {
                Title = Title,
                Owner = Owner,
                BlogPostsPageSize = BlogPostsPageSize,
                LatestBlogPostsCount = LatestBlogPostsCount
            };
        }

        private void MapFromSiteConfiguration(SiteConfiguration config)
        {
            Title = config.Title;
            Owner = config.Owner;
            BlogPostsPageSize = config.BlogPostsPageSize;
            LatestBlogPostsCount = config.LatestBlogPostsCount;
        }
    }
}
