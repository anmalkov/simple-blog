using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Repositories;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class ArticleModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;
        private readonly ISiteConfigurationRepository _configRepository;

        [BindProperty]
        [Required]
        public string Id { get; set; }
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Created { get; private set; }
        public List<string> Tags { get; private set; }
        public List<Article> RecommendedArticles { get; set; }
        public bool EnableComments { get; set; }
        public List<Comment> Comments { get; set; }
        public string Question { get; set; }

        [BindProperty]
        [Required]
        public string Hash { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Your name")]
        public string Author { get; set; }

        [BindProperty]
        [Required]
        public string Answer { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Your comment")]
        public string Comment { get; set; }

        public ArticleModel(IArticlesService articlesService, ISiteConfigurationRepository configRepository)
        {
            _articlesService = articlesService;
            _configRepository = configRepository;
        }

        public async Task OnGetAsync(string id)
        {
            var article = await _articlesService.GetAsync(id);
            if (article == null || (!article.Published && !User.Identity.IsAuthenticated))
            {
                Title = "404 Not Found";
                HtmlBody = "Unfortunately this blog post was not found.";
                Created = DateTime.Now;
                return;
            }
            Id = article.Id;
            Title = article.Title;
            HtmlBody = article.HtmlBody;
            Created = article.Created;
            Tags = new List<string>(article.Tags);

            await _articlesService.IncrementViewsCounterAsync(article.Id);

            var config = await _configRepository.GetAsync();
            if (config.RecommendedArticlesCount > 0)
            {
                RecommendedArticles = await _articlesService.GetRecommendedByTagsAsync(article.Id, article.Tags, config.RecommendedArticlesCount);
            }

            EnableComments = config.EnableComments;
            if (EnableComments)
            {
                var random = new Random();
                var x = random.Next(0, 5);
                var y = random.Next(0, 4);
                Hash = GetHash(article.Id, x + y);
                Question = $"Please calculate:  {x} + {y} = ?";
                Answer = "";
            }
            Comments = await _articlesService.GetCommentsAsync(article.Id);
            //Comments = new List<Comment>
            //{
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User 1", Created = DateTime.Now.AddDays(-3), Readed = false, Content = "This is the first comment about this article 1" },
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User Two", Created = DateTime.Now.AddDays(-4), Readed = false, Content = "This is the first comment about this article 2" },
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User 3", Created = DateTime.Now.AddDays(-5), Readed = false, Content = "This is the first comment about this article 3" },
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User4", Created = DateTime.Now.AddDays(-6), Readed = true, Content = "This is the first comment about this article 4" },
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User5", Created = DateTime.Now.AddDays(-7), Readed = true, Content = "This is the first comment about this article 5" },
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User6", Created = DateTime.Now.AddDays(-8), Readed = true, Content = "This is the first comment about this article 6" },
            //    new Comment {Id = Guid.NewGuid(), ArticleId = article.Id, Author = "User7", Created = DateTime.Now.AddDays(-9), Readed = true, Content = "This is the first comment about this article 7" },
            //};
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid && CheckHash(Id, Answer, Hash))
            {
                var comment = MapToComment();
                await _articlesService.CreateCommentAsync(comment);
            }

            return RedirectToPage("/blogarticle", new { id = Id });
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(Guid commentId)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _articlesService.DeleteCommentAsync(commentId);
            }
            return RedirectToPage("/blogarticle", new { id = Id });
        }

        public async Task<IActionResult> OnPostMarkCommentAsReadAsync(Guid commentId)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _articlesService.MarkCommentAsReadAsync(commentId);
            }
            return RedirectToPage("/blogarticle", new { id = Id });
        }

        private Comment MapToComment()
        {
            return new Comment
            {
                ArticleId = Id,
                Author = Author,
                Content = Comment
            };
        }

        private static string GetHash(string articleId, int number)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var text = $"{number} {articleId}";
                var data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
                var result = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    result.Append(data[i].ToString("x2"));
                }
                return result.ToString();
            }
        }

        private bool CheckHash(string articleId, string answer, string hash)
        {
            if (int.TryParse(answer, out int number))
            {
                var answerHash = GetHash(articleId, number);
                return answerHash == hash;
            }
            return false;
        }
    }
}
