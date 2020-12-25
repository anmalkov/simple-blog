using SimpleBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public interface IPagesService
    {
        Task<List<Page>> GetAllAsync();
        Task<Page> GetAsync(string id);
        Task<Page> GetIndexAsync();
        Task CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(string id);
    }
}
