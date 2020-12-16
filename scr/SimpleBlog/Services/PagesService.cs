using Markdig;
using Microsoft.Extensions.Logging;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public class PagesService : IPagesService
    {
        private readonly ILogger<PagesService> _logger;
        private readonly IPagesRepository _pagesRepository;

        public PagesService(ILogger<PagesService> logger, IPagesRepository pagesRepository)
        {
            _logger = logger;
            _pagesRepository = pagesRepository;
        }
        public async Task<List<Page>> GetAllAsync()
        {
            return await _pagesRepository.GetAllAsync();
        }

        public async Task CreateAsync(Page page)
        {
            page.HtmlBody = GetHtmlFromMarkdown(page.Body);
            await _pagesRepository.CreateAsync(page);
        }

        public async Task DeleteAsync(string id)
        {
            await _pagesRepository.DeleteAsync(id);
        }

        public async Task<Page> GetAsync(string id)
        {
            return await _pagesRepository.GetAsync(id);
        }

        public async Task UpdateAsync(Page page)
        {
            page.HtmlBody = GetHtmlFromMarkdown(page.Body);
            await _pagesRepository.UpdateAsync(page);
        }

        private string GetHtmlFromMarkdown(string markdownText)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UsePipeTables()
                .UseEmphasisExtras()
                .UseTaskLists()
                .UseBootstrap()
                .Build();

            return Markdown.ToHtml(markdownText, pipeline);
        }
    }
}
