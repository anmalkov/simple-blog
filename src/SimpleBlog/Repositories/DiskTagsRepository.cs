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
    public class DiskTagsRepository : ITagsRepository
    {
        private readonly ILogger<DiskTagsRepository> _logger;

        private ConcurrentDictionary<string, string> _tags;
        private readonly string _fileName;

        public DiskTagsRepository(ILogger<DiskTagsRepository> logger)
        {
            _logger = logger;
            _fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "tags.json");
        }

        public async Task<List<string>> GetAllAsync()
        {
            await LoadAsync();
            return _tags.Select(t => t.Value).OrderBy(t => t).ToList();
        }

        public async Task CreateAsync(string tag)
        {
            await LoadAsync();
            if (_tags.ContainsKey(tag))
            {
                return;
            }

            _tags.TryAdd(tag, tag);
            await SaveAsync();
        }

        public async Task DeleteAsync(string tag)
        {
            await LoadAsync();
            if (!_tags.ContainsKey(tag))
            {
                return;
            }

            _tags.TryRemove(tag, out string _);
            await SaveAsync();
        }


        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_tags);
            await File.WriteAllTextAsync(_fileName, json);
        }

        private async Task LoadAsync()
        {
            if (_tags != null)
            {
                return;
            }

            if (!File.Exists(_fileName))
            {
                _tags = new ConcurrentDictionary<string, string>();
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _tags = new ConcurrentDictionary<string, string>(JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(json));
        }
    }
}
