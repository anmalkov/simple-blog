﻿using Microsoft.Extensions.Logging;
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
    public class DiskArticlesRepository : IArticlesRepository
    {
        private readonly ILogger<DiskArticlesRepository> _logger;

        private ConcurrentDictionary<string, Article> _articles;
        private readonly string _fileName;

        public DiskArticlesRepository(ILogger<DiskArticlesRepository> logger)
        {
            _logger = logger;
            _fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "articles.json");
        }

        public async Task<List<Article>> GetAllAsync()
        {
            await LoadAsync();
            return _articles.Select(a => a.Value).OrderByDescending(a => a.Created).ToList();
        }

        public async Task<List<Article>> GetAllAsync(string tag)
        {
            await LoadAsync();
            return _articles.Where(a => a.Value.Tags.Contains(tag)).Select(a => a.Value).OrderByDescending(a => a.Created).ToList();
        }

        public async Task<int> GetArticlesCountForTagAsync(string tag)
        {
            await LoadAsync();
            return _articles.Where(a => a.Value.Tags.Contains(tag)).Count();
        }

        public async Task<List<Article>> GetAllAsync(int page, int pageSize, bool onlyPublished = true)
        {
            await LoadAsync();
            return _articles.Select(a => a.Value).Where(a => !onlyPublished || (onlyPublished && a.Published)).OrderByDescending(a => a.Created).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> GetTotalNumberOfPagesAsync(int pageSize, bool onlyPublished = true)
        {
            await LoadAsync();
            var count = _articles.Where(a => !onlyPublished || (onlyPublished && a.Value.Published)).Count();
            return count / pageSize + ((count % pageSize != 0) ? 1 : 0);
        }

        public async Task<Article> GetAsync(string id)
        {
            await LoadAsync();
            if (!_articles.ContainsKey(id))
            {
                return null;
            }
            return _articles[id];
        }

        public async Task ReleaseAsync(string id)
        {
            await LoadAsync();
            var article = await GetAsync(id);
            if (article == null)
            {
                return;
            }

            article.Published = true;
            await UpdateAsync(article);
        }

        public async Task CreateAsync(Article article)
        {
            await LoadAsync();
            if (_articles.ContainsKey(article.Id))
            {
                return;
            }

            _articles.TryAdd(article.Id, article);
            await SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await LoadAsync();
            if (!_articles.ContainsKey(id))
            {
                return;
            }

            _articles.TryRemove(id, out Article _);
            await SaveAsync();
        }

        public async Task UpdateAsync(Article article)
        {
            await LoadAsync();
            if (!_articles.ContainsKey(article.Id))
            {
                return;
            }

            _articles.TryRemove(article.Id, out Article _);
            _articles.TryAdd(article.Id, article);
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize<ConcurrentDictionary<string, Article>>(_articles);
            await File.WriteAllTextAsync(_fileName, json);
        }

        private async Task LoadAsync()
        {
            if (_articles != null)
            {
                return;
            }

            if (!File.Exists(_fileName))
            {
                _articles = new ConcurrentDictionary<string, Article>();
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _articles = new ConcurrentDictionary<string, Article>(JsonSerializer.Deserialize<ConcurrentDictionary<string, Article>>(json));
        }

        public async Task<bool> ExistsAsync(string id)
        {
            await LoadAsync();
            return _articles.ContainsKey(id);
        }

    }
}
