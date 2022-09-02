using Microsoft.Extensions.Logging;
using SimpleBlog.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleBlog.Repositories
{
    public class DiskCommentsRepository : ICommentsRepository
    {
        private readonly ILogger<DiskCommentsRepository> _logger;

        private ConcurrentDictionary<Guid, Comment> _comments;
        private readonly string _fileName;

        public DiskCommentsRepository(ILogger<DiskCommentsRepository> logger)
        {
            _logger = logger;
            _fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "comments.json");
        }

        public async Task<Comment> GetAsync(Guid id)
        {
            await LoadAsync();
            if (!_comments.ContainsKey(id))
            {
                return null;
            }
            return _comments[id];
        }

        public async Task<List<Comment>> GetAllAsync(string articleId)
        {
            await LoadAsync();
            return _comments.Where(c => c.Value.ArticleId == articleId).Select(c => c.Value).OrderBy(c => c.Created).ToList();
        }

        public async Task<List<Comment>> GetAllUnreadedAsync()
        {
            await LoadAsync();
            return _comments.Where(c => !c.Value.Readed).Select(c => c.Value).OrderBy(c => c.Created).ToList();
        }

        public async Task CreateAsync(Comment comment)
        {
            await LoadAsync();
            if (_comments.ContainsKey(comment.Id))
            {
                return;
            }

            _comments.TryAdd(comment.Id, comment);
            await SaveAsync();
        }

        public async Task MarkAsReadedAsync(Guid id)
        {
            await LoadAsync();
            if (!_comments.ContainsKey(id))
            {
                return;
            }

            var comment = _comments[id];
            comment.Readed = true;
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await LoadAsync();
            if (!_comments.ContainsKey(id))
            {
                return;
            }

            _comments.TryRemove(id, out Comment _);
            await SaveAsync();
        }

        public async Task DeleteAllAsync(string articleId)
        {
            await LoadAsync();
            var comments = await GetAllAsync(articleId);
            foreach (var comment in comments)
            {
                _comments.TryRemove(comment.Id, out Comment _);
            }
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize<ConcurrentDictionary<Guid, Comment>>(_comments);
            await File.WriteAllTextAsync(_fileName, json);
        }

        private async Task LoadAsync()
        {
            if (_comments != null)
            {
                return;
            }

            if (!File.Exists(_fileName))
            {
                _comments = new ConcurrentDictionary<Guid, Comment>();
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _comments = new ConcurrentDictionary<Guid, Comment>(JsonSerializer.Deserialize<ConcurrentDictionary<Guid, Comment>>(json));
        }

    }
}
