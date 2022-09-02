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
        private readonly ICommentsRepository _commentsRepository;

        public ArticlesService(ILogger<ArticlesService> logger, IArticlesRepository articlesRepository, IArticleInfoRepository articleInfoRepository, 
            ITagsRepository tagsRepository, ICommentsRepository commentsRepository)
        {
            _logger = logger;
            _articlesRepository = articlesRepository;
            _articleInfoRepository = articleInfoRepository;
            _tagsRepository = tagsRepository;
            _commentsRepository = commentsRepository;
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
            await DeleteCommentsAsync(id);
            await _articleInfoRepository.DeleteAsync(id);
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

        public async Task<List<Article>> GetRecommendedByTagsAsync(string excludeArticleId, List<string> tags, int articlesCount)
        {
            var articlesRank = new Dictionary<string, ArticleRank>();
            foreach (var tag in tags)
            {
                var articlesByTag = await GetAllAsync(tag);
                foreach (var article in articlesByTag.Where(a => a.Published && a.Id != excludeArticleId).ToList())
                {
                    if (articlesRank.ContainsKey(article.Id))
                    {
                        articlesRank[article.Id].TagsRank++;
                    }
                    else
                    {
                        articlesRank.Add(article.Id, new ArticleRank { TagsRank = 1, Created = article.Created });
                    }
                }
            }

            var articles = new List<Article>();
            foreach (var id in articlesRank.OrderByDescending(d => d.Value.TagsRank).ThenByDescending(d => d.Value.Created).Select(d => d.Key).Take(articlesCount).ToList())
            {
                articles.Add(await _articlesRepository.GetAsync(id));
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

        
        public async Task<List<Comment>> GetCommentsAsync(string id)
        {
            return await _commentsRepository.GetAllAsync(id);
        }

        public async Task<List<Comment>> GetUnreadedCommentsAsync()
        {
            return await _commentsRepository.GetAllUnreadedAsync();
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            comment.Id = Guid.NewGuid();
            comment.Created = DateTime.Now;
            comment.Readed = false;
            await _commentsRepository.CreateAsync(comment);
            await _articleInfoRepository.IncrementCommensCounterAsync(comment.ArticleId);
        }

        private async Task DeleteCommentsAsync(string articleId)
        {
            await _commentsRepository.DeleteAllAsync(articleId);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var articleId = (await _commentsRepository.GetAsync(id))?.ArticleId;
            await _commentsRepository.DeleteAsync(id);
            if (!string.IsNullOrEmpty(articleId))
            {
                await _articleInfoRepository.DecrementCommensCounterAsync(articleId);
            }
        }

        public async Task MarkCommentAsReadAsync(Guid id)
        {
            await _commentsRepository.MarkAsReadedAsync(id);
        }
    }
}
