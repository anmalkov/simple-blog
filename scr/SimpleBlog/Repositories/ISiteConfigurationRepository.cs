using SimpleBlog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface ISiteConfigurationRepository
    {
        Task<SiteConfiguration> GetAsync();
        Task UpdateAsync(SiteConfiguration config);

        Task<MenuItem> GetMenuItemAsync(string title);
        Task CreateMenuItemAsync(MenuItem menuItem);
        Task UpdateMenuItemAsync(string oldTitle, MenuItem menuItem);
        Task DeleteAsync(string title);
        Task<List<MenuItem>> GetAllMenuItemsAsync();
    }
}
