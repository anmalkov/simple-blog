using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleBlog.Model;
using SimpleBlog.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminCommentsModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IArticlesService _articlesService;
        public List<Comment> Comments { get; set; }

        public AdminCommentsModel(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        public async Task OnGet()
        {
            Comments = await _articlesService.GetUnreadedCommentsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _articlesService.DeleteCommentAsync(id);
            return RedirectToPage("/admincomments");
        }
        public async Task<IActionResult> OnPostMarkAsReadAsync(Guid id)
        {
            await _articlesService.MarkCommentAsReadAsync(id);
            return RedirectToPage("/admincomments");
        }
    }
}
