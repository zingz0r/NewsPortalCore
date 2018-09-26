using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Models.Entiy
{
    public class Article
    {

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Summary { get; set; }
        
        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsFeatured { get; set; }

        // Foreign key
        [Required]
        public int UserId { get; set; }

        public User Author { get; set; }
    }
}
