using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Models.Entiy
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        public byte[] LargeImageData { get; set; }
        public byte[] SmallImageData { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
