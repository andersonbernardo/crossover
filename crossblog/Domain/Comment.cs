using System;

namespace crossblog.Domain
{
    public class Comment : BaseEntity
    {
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
        
        public string Email { get; set; }        
        
        public string Title { get; set; }
        
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool Published { get; set; }
    }
}