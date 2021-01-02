using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public interface IImagesRepository
    {
        Task<List<string>> GetAllAsync(string articleId);
        Task UploadAsync(string articleId, IFormFile formFile);
        Task DeleteAsync(string articleId, string fileName);

        Task UploadFaviconAsync(IFormFile formFile);
        Task<bool> FaviconExists();
    }
}
