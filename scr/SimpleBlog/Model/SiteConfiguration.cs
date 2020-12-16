using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Model
{
    public class SiteConfiguration
    {
        public string Title { get; set; }
        public string Owner { get; set; }
        public int BlogPostsPageSize { get; set; }
    }
}
