using Microsoft.Extensions.Logging;
using SimpleBlog.Model;
using System.IO;
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
                    BlogPostsPageSize = 20
                };
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _siteConfiguration = JsonSerializer.Deserialize<SiteConfiguration>(json);
        }
    }
}
