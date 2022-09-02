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

namespace SimpleBlog.Repositories
{
    public class DiskArticleInfoRepository : IArticleInfoRepository
    {
        private readonly ILogger<DiskArticleInfoRepository> _logger;

        private ConcurrentDictionary<string, ArticleInfo> _articleInfos;
        private readonly string _fileName;


        public DiskArticleInfoRepository(ILogger<DiskArticleInfoRepository> logger)
        {
            _logger = logger;
            _fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "articleinfos.json");
        }


        public async Task IncrementViewsCounterAsync(string articleId)
        {
            await LoadAsync();
            if (!_articleInfos.ContainsKey(articleId))
            {
                _articleInfos.TryAdd(articleId, new ArticleInfo
                {
                    ArticleId = articleId,
                    ViewsCount = 1,
                    CommentsCount = 0
                });
            }
            else
            {
                var articleInfo = _articleInfos[articleId];
                articleInfo.ViewsCount++;
            }
            await SaveAsync();
        }

        public async Task IncrementCommensCounterAsync(string articleId)
        {
            await LoadAsync();
            if (!_articleInfos.ContainsKey(articleId))
            {
                _articleInfos.TryAdd(articleId, new ArticleInfo
                {
                    ArticleId = articleId,
                    ViewsCount = 0,
                    CommentsCount = 1
                });
            }
            else
            {
                var articleInfo = _articleInfos[articleId];
                articleInfo.CommentsCount++;
            }
            await SaveAsync();
        }

        public async Task DecrementCommensCounterAsync(string articleId)
        {
            await LoadAsync();
            if (!_articleInfos.ContainsKey(articleId))
            {
                _articleInfos.TryAdd(articleId, new ArticleInfo
                {
                    ArticleId = articleId,
                    ViewsCount = 0,
                    CommentsCount = 0
                });
            }
            else
            {
                var articleInfo = _articleInfos[articleId];
                if (articleInfo.CommentsCount == 0)
                {
                    return;
                }
                articleInfo.CommentsCount--;
            }
            await SaveAsync();
        }

        public async Task<List<ArticleInfo>> GetAllAsync()
        {
            await LoadAsync();
            return _articleInfos.Select(a => a.Value).ToList();
        }

        public async Task<List<ArticleInfo>> GetAllOrderedByViewsCountAsync(int itemsCount)
        {
            await LoadAsync();
            return _articleInfos.Select(a => a.Value).OrderByDescending(a => a.ViewsCount).Take(itemsCount).ToList();
        }

        public async Task<ArticleInfo> GetAsync(string articleId)
        {
            await LoadAsync();
            if (!_articleInfos.ContainsKey(articleId))
            {
                return null;
            }
            return _articleInfos[articleId];
        }

        public async Task DeleteAsync(string articleId)
        {
            await LoadAsync();
            if (!_articleInfos.ContainsKey(articleId))
            {
                return;
            }

            _articleInfos.TryRemove(articleId, out ArticleInfo _);
            await SaveAsync();
        }

        public async Task<int> GetCommentsCountAsync(string articleId)
        {
            return (await GetAsync(articleId)).CommentsCount;
        }

        public async Task<long> GetViewsCountAsync(string articleId)
        {
            return (await GetAsync(articleId)).ViewsCount;
        }


        private async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_articleInfos);
            await File.WriteAllTextAsync(_fileName, json);
        }

        private async Task LoadAsync()
        {
            if (_articleInfos != null)
            {
                return;
            }

            if (!File.Exists(_fileName))
            {
                _articleInfos = new ConcurrentDictionary<string, ArticleInfo>();
                return;
            }

            var json = await File.ReadAllTextAsync(_fileName);
            _articleInfos = new ConcurrentDictionary<string, ArticleInfo>(JsonSerializer.Deserialize<ConcurrentDictionary<string, ArticleInfo>>(json));
        }
    }
}
