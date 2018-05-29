using System;

namespace crossblog.Domain
{
    public class Article : BaseEntity
    {        
        public string Title { get; set; }
        
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool Published { get; set; }
    }
}