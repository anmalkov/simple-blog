using Microsoft.Extensions.Logging;
using SimpleBlog.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public class DiskSiteConfigurationRepository : ISiteConfigurationRepository
    {
        private readonly ILogger<DiskSiteConfigurationRepository> _logger;

        private SiteConfiguration _siteConfiguration;
        private readonly string _fileName;

        public DiskSiteConfigurationRepository(ILogger<DiskSiteConfigurationRepository> logger)
        {
            _logger = logger;
            _fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");
        }

        public async Task<SiteConfiguration> GetAsync()
        {
            await LoadAsync();
            return new SiteConfiguration
            {
                Title = _siteConfiguration.Title,
                Owner = _siteConfiguration.Owner,
                BlogPostsPageSize = _siteConfiguration.BlogPostsPageSize
            };
        }

        public async Task UpdateAsync(SiteConfiguration config)
        {
            await LoadAsync();

            _siteConfiguration.Title = config.Title;
            _siteConfiguration.Owner = config.Owner;
            _siteConfiguration.BlogPostsPageSize = config.BlogPostsPageSize;

            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize<SiteConfiguration>(_siteConfiguration);
            await File.WriteAllTextAsync(_fileName, json);
        }

        private async Task LoadAsync()
        {
            if (_siteConfiguration != null)
            {
                return;
            }

            if (!File.Exists(_fileName))
            {
                _siteConfiguration = new SiteConfiguration
                {
                    Title = "Simple Blog",
                    Owner = "Simple Blog",
                    BlogPostsPageSize = 20,
                    MenuItems = new ConcurrentDictionary<string, MenuItem>()
                };
                _siteConfiguration.MenuItems.TryAdd("blog", new MenuItem { Order = 10, Title = "blog", Url = "/blog" });
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _siteConfiguration = JsonSerializer.Deserialize<SiteConfiguration>(json);
        }

        public async Task<MenuItem> GetMenuItemAsync(string title)
        {
            await LoadAsync();
            if (!_siteConfiguration.MenuItems.ContainsKey(title))
            {
                return null;
            }
            return _siteConfiguration.MenuItems[title];
        }

        public async Task CreateMenuItemAsync(MenuItem menuItem)
        {
            await LoadAsync();

            if (_siteConfiguration.MenuItems.ContainsKey(menuItem.Title))
            {
                return;
            }

            _siteConfiguration.MenuItems.TryAdd(menuItem.Title, menuItem);

            await SaveAsync();
        }


        public async Task UpdateMenuItemAsync(string oldTitle, MenuItem menuItem)
        {
            await LoadAsync();

            if (!_siteConfiguration.MenuItems.ContainsKey(oldTitle) || _siteConfiguration.MenuItems.ContainsKey(menuItem.Title))
            {
                return;
            }

            _siteConfiguration.MenuItems.TryRemove(oldTitle, out MenuItem _);
            _siteConfiguration.MenuItems.TryAdd(menuItem.Title, menuItem);

            await SaveAsync();
        }

        public async Task DeleteAsync(string title)
        {
            await LoadAsync();
            if (!_siteConfiguration.MenuItems.ContainsKey(title))
            {
                return;
            }

            _siteConfiguration.MenuItems.TryRemove(title, out MenuItem _);

            await SaveAsync();
        }

        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            await LoadAsync();
            return _siteConfiguration.MenuItems.Select(m => m.Value).OrderBy(m => m.Order).ToList();
        }
    }
}
