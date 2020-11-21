using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    public class ArticleModel : PageModel
    {
        private readonly IArticlesService _articlesService;

        public string Title { get; set; }
        public string Body { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}")]
        public DateTime Created { get; set; }
        public List<string> Tags { get; set; }

        public ArticleModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        public async Task OnGetAsync(string id)
        {
            var article = await _articlesService.GetAsync(id);
            if (article == null || (!article.Published && !User.Identity.IsAuthenticated))
            {
                Title = "404 Not Found";
                Body = "Unfortunately this blog post was not found.";
                Created = DateTime.Now;
                return;
            }
            Title = article.Title;
            Body = article.Body;
            Created = article.Created;
            Tags = new List<string>(article.Tags);
        }
    }
}
