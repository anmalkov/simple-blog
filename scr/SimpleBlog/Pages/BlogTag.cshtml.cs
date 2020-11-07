using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleBlog.Pages
{
    public class BlogTagModel : PageModel
    {
        public string Tag { get; set; }
        
        public void OnGet(string tag)
        {
            Tag = tag;
        }
    }
}
