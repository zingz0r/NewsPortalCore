using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Data.Entity
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
