using Markdig;
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
        private readonly IArticleInfoRepository _articleInfoRepository;
        private readonly ITagsRepository _tagsRepository;

        public ArticlesService(ILogger<ArticlesService> logger, IArticlesRepository articlesRepository, IArticleInfoRepository articleInfoRepository, ITagsRepository tagsRepository)
        {
            _logger = logger;
            _articlesRepository = articlesRepository;
            _articleInfoRepository = articleInfoRepository;
            _tagsRepository = tagsRepository;
        }

        public async Task CreateAsync(Article article)
        {
            article.Created = DateTime.Now;
            article.HtmlBody = GetHtmlFromMarkdown(article.Body);
            article.Tags = article.Tags.Select(t => t.Trim()).ToList();

            await _articlesRepository.CreateAsync(article);
            await CreateTagsAsync(article.Tags);
        }

        public async Task DeteleAsync(string id)
        {
            var article = await GetAsync(id);
            await _articlesRepository.DeleteAsync(id);
            await DeleteTagsAsync(article.Tags);
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _articlesRepository.GetAllAsync();
        }

        public async Task<List<Article>> GetAllAsync(string tag)
        {
            return await _articlesRepository.GetAllAsync(tag);
        }

        public async Task<List<Article>> GetAllAsync(int page, int pageSize)
        {
             return await _articlesRepository.GetAllAsync(page, pageSize);
        }

        public async Task<List<Article>> GetLatestAsync(int articlesCount)
        {
            return await GetAllAsync(1, articlesCount);
        }

        public async Task<List<Article>> GetMostPopularAsync(int articlesCount)
        {
            var articleInfos = await _articleInfoRepository.GetAllOrderedByViewsCountAsync(articlesCount);
            var articles = new List<Article>();
            foreach (var articleInfo in articleInfos)
            {
                var article = await _articlesRepository.GetAsync(articleInfo.ArticleId);
                if (article.Published)
                {
                    articles.Add(article);
                }
            }
            return articles;
        }

        public async Task<int> GetTotalNumberOfPagesAsync(int pageSize)
        {
            return await _articlesRepository.GetTotalNumberOfPagesAsync(pageSize);
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
            var oldArticle = await _articlesRepository.GetAsync(article.Id);
            if (oldArticle == null)
            {
                return;
            }

            article.Tags = article.Tags.Select(t => t.Trim()).ToList();

            var tagsToDelete = oldArticle.Tags.Except(article.Tags).ToList();
            var tagsToCreate = article.Tags.Except(oldArticle.Tags).ToList();

            article.Created = oldArticle.Created;
            article.Updated = DateTime.Now;
            article.HtmlBody = GetHtmlFromMarkdown(article.Body);

            await _articlesRepository.UpdateAsync(article);

            await CreateTagsAsync(tagsToCreate);
            await DeleteTagsAsync(tagsToDelete);
        }


        public async Task<List<string>> GetAllTagsAsync()
        {
            return await _tagsRepository.GetAllAsync();
        }

        private async Task CreateTagsAsync(List<string> tags)
        {
            foreach (var tag in tags)
            {
                await _tagsRepository.CreateAsync(tag.Trim());
            }
        }

        private async Task DeleteTagsAsync(List<string> tags)
        {
            foreach (var tag in tags)
            {
                if (await _articlesRepository.GetArticlesCountForTagAsync(tag.Trim()) == 0)
                {
                    await _tagsRepository.DeleteAsync(tag.Trim());
                }
            }
        }


        public async Task<List<ArticleInfo>> GetAllArticleInfosAsync()
        {
            return await _articleInfoRepository.GetAllAsync();
        }

        public async Task IncrementViewsCounterAsync(string id)
        {
            await _articleInfoRepository.IncrementViewsCounterAsync(id);
        }


        private static string GetHtmlFromMarkdown(string markdownText)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UsePipeTables()
                .UseEmphasisExtras()
                .UseTaskLists()
                .UseBootstrap()
                .UseMediaLinks()
                .Build();

            return Markdown.ToHtml(markdownText, pipeline);
        }
    }
}
