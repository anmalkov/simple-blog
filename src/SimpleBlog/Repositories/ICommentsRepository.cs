using SimpleBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface ICommentsRepository
    {
        Task<List<Comment>> GetAllUnreadedAsync();
        Task<List<Comment>> GetAllAsync(string articleId);
        Task<Comment> GetAsync(Guid id);
        Task CreateAsync(Comment comment);
        Task MarkAsReadedAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task DeleteAllAsync(string articleId);
    }
}
