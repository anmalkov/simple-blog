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

        public async Task OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            var config = MapToSiteConfiguration();

            await _configRepository.UpdateAsync(config);

            MenuItems = await _configRepository.GetAllMenuItemsAsync();
        }

        private SiteConfiguration MapToSiteConfiguration()
        {
            return new SiteConfiguration
            {
                Title = Title,
                Owner = Owner,
                BlogPostsPageSize = BlogPostsPageSize
            };
        }

        private void MapFromSiteConfiguration(SiteConfiguration config)
        {
            Title = config.Title;
            Owner = config.Owner;
            BlogPostsPageSize = config.BlogPostsPageSize;
        }
    }
}