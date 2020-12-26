using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.ViewModels
{
    public class ArticleViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public bool Published { get; set; }
        public long ViewsCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
