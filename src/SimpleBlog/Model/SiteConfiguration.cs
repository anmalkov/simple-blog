using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Model
{
    public class SiteConfiguration
    {
        public string Title { get; set; }
        public string Owner { get; set; }
        public int LatestBlogPostsCount { get; set; }
        public int BlogPostsPageSize { get; set; }
        public bool EnableClientSideTelemetry { get; set; }
        public int RecommendedArticlesCount { get; set; }
        public bool EnableComments { get; set; }
        public ConcurrentDictionary<string, MenuItem> MenuItems { get; set; }
    }
}
