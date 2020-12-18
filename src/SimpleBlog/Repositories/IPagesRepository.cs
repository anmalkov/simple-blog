using SimpleBlog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface IPagesRepository
    {
        Task<List<Page>> GetAllAsync();
        Task<Page> GetAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(string id);
    }
}
