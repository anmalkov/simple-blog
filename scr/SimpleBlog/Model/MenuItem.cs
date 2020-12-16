using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Model
{
    public class MenuItem
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
