using SimpleBlog.Model;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface ISiteConfigurationRepository
    {
        Task<SiteConfiguration> GetAsync();
        Task UpdateAsync(SiteConfiguration config);
    }
}
