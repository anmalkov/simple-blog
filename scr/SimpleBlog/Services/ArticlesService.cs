using Microsoft.Extensions.Logging;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public class ArticlesService : IArticlesService
    {
        private readonly ILogger<ArticlesService> _logger;
        private readonly IArticlesRepository _articlesRepository;
        public ArticlesService(ILogger<ArticlesService> logger, IArticlesRepository articlesRepository)
        {
            _logger = logger;
            _articlesRepository = articlesRepository;
        }

        public async Task CreateAsync(Article article)
        {
            await _articlesRepository.CreateAsync(article);
        }

        public async Task DeteleAsync(string id)
        {
            await _articlesRepository.DeleteAsync(id);
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _articlesRepository.GetAllAsync();
        }

        public async Task<List<Article>> GetAllAsync(string tag)
        {
            return await _articlesRepository.GetAllAsync(tag);
        }

        public async Task<Article> GetAsync(string id)
        {
            return await _articlesRepository.GetAsync(id);
        }

        public async Task ReleaseAsync(string id)
        {
            await _articlesRepository.ReleaseAsync(id);
        }

        public async Task UpdateAsync(Article article)
        {
            await _articlesRepository.UpdateAsync(article);
        }
    }
}
