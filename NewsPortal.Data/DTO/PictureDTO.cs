using System;

namespace NewsPortal.Data.DTO
{
    public class PictureDTO : ICloneable
    {
        public int Id { get; set; }
        public byte[] LargeImageData { get; set; }
        public byte[] SmallImageData { get; set; }
        public int ArticleId { get; set; }
        public ArticleDTO Article { get; set; }
        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
