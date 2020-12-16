using SimpleBlog.Model;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface IPagesRepository
    {
        Task<Page> GetAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(string id);
    }
}
