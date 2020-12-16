using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Model;
using SimpleBlog.Services;

namespace SimpleBlog.Pages
{
    [Authorize]
    public class AdminPagesModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IPagesService _pagesService;
        public List<Page> Pages { get; set; }

        public AdminPagesModel(IPagesService pagesService)
        {
            _pagesService = pagesService;
        }

        public async Task OnGet()
        {
            Pages = await _pagesService.GetAllAsync();
        }
    }
}
