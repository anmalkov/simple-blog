using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleBlog.Pages
{
    public class AdminPostModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Text { get; set; }

        public void OnGet()
        {
        }
        public void OnPost()
        {
        }
    }
}
