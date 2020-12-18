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
    public class DiskPagesRepository : IPagesRepository
    {
        private readonly ILogger<DiskPagesRepository> _logger;

        private ConcurrentDictionary<string, Page> _pages;
        private readonly string _fileName;

        public DiskPagesRepository(ILogger<DiskPagesRepository> logger)
        {
            _logger = logger;
            _fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "pages.json");
        }

        public async Task<List<Page>> GetAllAsync()
        {
            await LoadAsync();
            return _pages.Select(p => p.Value).OrderByDescending(p => p.Id).ToList();
        }

        public async Task<Page> GetAsync(string id)
        {
            await LoadAsync();
            if (!_pages.ContainsKey(id))
            {
                return null;
            }
            return _pages[id];
        }

        public async Task CreateAsync(Page page)
        {
            await LoadAsync();
            if (_pages.ContainsKey(page.Id))
            {
                return;
            }

            _pages.TryAdd(page.Id, page);
            await SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await LoadAsync();
            if (!_pages.ContainsKey(id))
            {
                return;
            }

            _pages.TryRemove(id, out Page _);
            await SaveAsync();
        }

        public async Task UpdateAsync(Page page)
        {
            await LoadAsync();
            if (!_pages.ContainsKey(page.Id))
            {
                return;
            }

            _pages.TryRemove(page.Id, out Page _);
            _pages.TryAdd(page.Id, page);
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize<ConcurrentDictionary<string, Page>>(_pages);
            await File.WriteAllTextAsync(_fileName, json);
        }

        private async Task LoadAsync()
        {
            if (_pages != null)
            {
                return;
            }

            if (!File.Exists(_fileName))
            {
                _pages = new ConcurrentDictionary<string, Page>();
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _pages = new ConcurrentDictionary<string, Page>(JsonSerializer.Deserialize<ConcurrentDictionary<string, Page>>(json));
        }

        public async Task<bool> ExistsAsync(string id)
        {
            await LoadAsync();
            return _pages.ContainsKey(id);
        }
    }
}
