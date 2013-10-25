using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search_engine
{
    class Document
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentAll
        {
            get
            {
                return Title + Content;
            }
        }
    }
}
