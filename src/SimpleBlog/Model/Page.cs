using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Model
{
    public class Page
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public string Title { get; set; }
    }
}
