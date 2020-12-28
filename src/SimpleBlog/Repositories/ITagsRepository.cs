using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface ITagsRepository
    {
        Task<List<string>> GetAllAsync();
        Task CreateAsync(string tag);
        Task DeleteAsync(string tag);
    }
}
