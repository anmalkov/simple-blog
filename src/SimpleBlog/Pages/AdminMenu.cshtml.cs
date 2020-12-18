using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Model;
using SimpleBlog.Repositories;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminMenuModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly ISiteConfigurationRepository _configRepository;

        [BindProperty]
        [Required]
        [Display(Name = "Order")]
        public int Order { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Title")]
        public string MenuTitle { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Url")]
        public string MenuUrl { get; set; }

        public AdminMenuModel(ISiteConfigurationRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task OnGet(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return;
            }

            var menuItem = await _configRepository.GetMenuItemAsync(title);
            MapFromMenuItem(menuItem);
        }

        public async Task<IActionResult> OnPost(string title)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menuItem = MapToMenuItem();

            if (string.IsNullOrEmpty(title))
            {
                await _configRepository.CreateMenuItemAsync(menuItem);
            }
            else
            {
                await _configRepository.UpdateMenuItemAsync(title, menuItem);
            }

            return RedirectToPage("/adminconfig");
        }

        private MenuItem MapToMenuItem()
        {
            return new MenuItem
            {
                Order = Order,
                Title = MenuTitle,
                Url = MenuUrl
            };
        }

        private void MapFromMenuItem(MenuItem menuItem)
        {
            Order = menuItem.Order;
            MenuTitle = menuItem.Title;
            MenuUrl = menuItem.Url;
        }
    }
}
