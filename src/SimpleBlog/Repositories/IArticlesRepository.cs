using SimpleBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface IArticlesRepository
    {
        Task<List<Article>> GetAllAsync();
        Task<List<Article>> GetAllAsync(string tag);
        Task<int> GetArticlesCountForTagAsync(string tag);
        Task<List<Article>> GetAllAsync(int page, int pageSize, bool onlyPublished = true);
        Task<int> GetTotalNumberOfPagesAsync(int pageSize, bool onlyPublished = true);
        Task<Article> GetAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task CreateAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeleteAsync(string id);
        Task ReleaseAsync(string id);
    }
}
