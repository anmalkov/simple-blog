using SimpleBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface IArticleInfoRepository
    {
        Task IncrementViewsCounterAsync(string articleId);
        Task<List<ArticleInfo>> GetAllAsync();
        Task<List<ArticleInfo>> GetAllOrderedByViewsCountAsync(int itemsCount);
        Task<ArticleInfo> GetAsync(string articleId);
        Task<long> GetViewsCountAsync(string articleId);
        Task<int> GetCommentsCountAsync(string articleId);
    }
}
