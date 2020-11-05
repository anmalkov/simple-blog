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
        Task<Article> GetAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task CreateAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeleteAsync(string id);
        Task ReleaseAsync(string id);
    }
}
