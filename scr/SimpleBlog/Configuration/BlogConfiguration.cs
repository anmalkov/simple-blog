using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Configuration
{
    public class BlogConfiguration
    {
        public const string SectionName = "SimpleBlog";

        public string Title { get; set; }
        public string Owner { get; set; }
    }
}
