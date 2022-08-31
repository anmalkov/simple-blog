using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Repositories;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminConfigModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private const string AppInsightsConnectionStringEnvironmentVariableName = "APPLICATIONINSIGHTS_CONNECTION_STRING";

        private readonly ISiteConfigurationRepository _configRepository;
        private readonly IImagesRepository _imagesRepository;

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

        [BindProperty]
        [Required]
        [Display(Name = "Enable client-side telemetry")]
        public bool EnableClientSideTelemetry { get; set; }
        public bool InstrumentationKeyProvided => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(AppInsightsConnectionStringEnvironmentVariableName));

        [BindProperty]
        [Required]
        [Display(Name = "How many recommended articles to show for each article")]
        public int RecommendedArticlesCount { get; set; }

        public List<MenuItem> MenuItems { get; set; }

        public bool FaviconExists { get;  private set; }

        [BindProperty]
        public List<IFormFile> UploadFavicons { get; set; }


        public AdminConfigModel(ISiteConfigurationRepository configRepository, IImagesRepository imagesRepository)
        {
            _configRepository = configRepository;
            _imagesRepository = imagesRepository;
        }

        public async Task OnGet()
        {
            var config = await _configRepository.GetAsync();
            MapFromSiteConfiguration(config);

            MenuItems = await _configRepository.GetAllMenuItemsAsync();

            FaviconExists = await _imagesRepository.FaviconExists();
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
                MenuItems = await _configRepository.GetAllMenuItemsAsync();
                FaviconExists = await _imagesRepository.FaviconExists();
                return Page();
            }

            var config = MapToSiteConfiguration();

            await _configRepository.UpdateAsync(config);

            return RedirectToPage("/adminconfig");
        }

        public async Task<IActionResult> OnPostFaviconAsync(string id)
        {
            foreach (var image in UploadFavicons)
            {
                await _imagesRepository.UploadFaviconAsync(image);
            }
            return RedirectToPage("/adminconfig", new { id });
        }

        private SiteConfiguration MapToSiteConfiguration()
        {
            return new SiteConfiguration
            {
                Title = Title,
                Owner = Owner,
                BlogPostsPageSize = BlogPostsPageSize,
                LatestBlogPostsCount = LatestBlogPostsCount,
                EnableClientSideTelemetry = EnableClientSideTelemetry,
                RecommendedArticlesCount = RecommendedArticlesCount
            };
        }

        private void MapFromSiteConfiguration(SiteConfiguration config)
        {
            Title = config.Title;
            Owner = config.Owner;
            BlogPostsPageSize = config.BlogPostsPageSize;
            LatestBlogPostsCount = config.LatestBlogPostsCount;
            EnableClientSideTelemetry = config.EnableClientSideTelemetry;
            RecommendedArticlesCount = config.RecommendedArticlesCount;
        }
    }
}
