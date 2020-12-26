using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Model
{
    public class ArticleInfo
    {
        public string ArticleId { get; set; }
        public long ViewsCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
