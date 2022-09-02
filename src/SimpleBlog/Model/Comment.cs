using System;

namespace SimpleBlog.Model
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public string ArticleId { get; set; }
        public bool Readed { get; set; }
    }
}
