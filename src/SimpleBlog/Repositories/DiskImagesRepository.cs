using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleBlog.Repositories
{
    public class DiskImagesRepository : IImagesRepository
    {
        private readonly ILogger<DiskImagesRepository> _logger;

        private readonly string _directoryName;

        public DiskImagesRepository(ILogger<DiskImagesRepository> logger)
        {
            _logger = logger;
            _directoryName = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data"), "images");
        }

        public Task<List<string>> GetAllAsync(string articleId)
        {
            var articleDirectory = Path.Combine(_directoryName, articleId);
            if (!Directory.Exists(articleDirectory))
            {
                return Task.FromResult(new List<string>());
            }
            var images = Directory.GetFiles(articleDirectory).ToList();
            return Task.FromResult(images);
        }

        public async Task UploadAsync(string articleId, IFormFile formFile)
        {
            var articleDirectory = Path.Combine(_directoryName, articleId);
            if (!Directory.Exists(articleDirectory))
            {
                Directory.CreateDirectory(articleDirectory);
            }

            var fileName = Path.Combine(articleDirectory, formFile.FileName);
            using var fileStream = new FileStream(fileName, FileMode.Create);
            await formFile.CopyToAsync(fileStream);
        }
        public Task DeleteAsync(string articleId, string fileName)
        {
            var articleDirectory = Path.Combine(_directoryName, articleId);
            File.Delete(Path.Combine(articleDirectory, fileName));
            return Task.CompletedTask;
        }
    }
}
