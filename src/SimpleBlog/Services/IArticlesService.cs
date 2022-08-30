using SimpleBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public interface IArticlesService
    {
        Task<List<Article>> GetAllAsync();
        Task<List<Article>> GetAllAsync(string tag);
        Task<List<Article>> GetAllAsync(int page, int pageSize);
        Task<List<Article>> GetLatestAsync(int articlesCount);
        Task<List<Article>> GetMostPopularAsync(int articlesCount);
        Task<List<Article>> GetRecommendedByTagsAsync(List<string> tags, int articlesCount);
        Task<int> GetTotalNumberOfPagesAsync(int pageSize);
        Task<Article> GetAsync(string id);
        Task CreateAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeteleAsync(string id);
        Task ReleaseAsync(string id);

        Task<List<string>> GetAllTagsAsync();

        Task<List<ArticleInfo>> GetAllArticleInfosAsync();
        Task IncrementViewsCounterAsync(string id);
    }
}
