using System;
using System.Collections.Generic;
using NewsPortal.Data.Entity;

namespace NewsPortal.Data.DTO
{
    public class ArticleDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User Author { get; set; }
        public IList<Picture> Images { get; set; }
    }
}