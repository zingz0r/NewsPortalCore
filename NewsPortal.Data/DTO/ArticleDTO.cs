using System;
using System.Collections;
using System.Collections.ObjectModel;
using NewsPortal.Data.Entity;

namespace NewsPortal.Data.DTO
{
    public class ArticleDTO : ICloneable
    {

        public ArticleDTO(ArticleDTO other)
        {
            Id = other.Id;
            Title = other.Title;
            Summary = other.Summary;
            Text = other.Text;
            IsFeatured = other.IsFeatured;
            Date = other.Date;
            UserId = other.UserId;
            Author = other.Author;
            Images = other.Images;
        }

        public ArticleDTO()
        {
            Images = new ObservableCollection<PictureDTO>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User Author { get; set; }
        public ObservableCollection<PictureDTO> Images { get; set; }


        public object Clone()
        {
            ArticleDTO clonedItem = base.MemberwiseClone() as ArticleDTO;
            if (clonedItem != null)
            {
                clonedItem.Images = new ObservableCollection<PictureDTO>();

                if (Images != null)
                    foreach (PictureDTO item in (IEnumerable) Images)
                    {
                        clonedItem.Images.Add(item.Clone() as PictureDTO);
                    }

            }
            return clonedItem ?? throw new InvalidOperationException();

        }

        public override bool Equals(object obj)
        {
            return (obj is ArticleDTO dto) && Id == dto.Id;
        }

        public static ArticleDTO ConvertArticleToDTO(Article article)
        {
            return new ArticleDTO
            {
                Id = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Text = article.Text,
                IsFeatured = article.IsFeatured,
                Date = article.Date,
                UserId = article.UserId,
                Author = article.Author
            };
        }
        public static Article ConvertDTOToArticle(ArticleDTO article)
        {
            return new Article
            {
                Id = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Text = article.Text,
                IsFeatured = article.IsFeatured,
                Date = article.Date,
                UserId = article.UserId,
                Author = article.Author
            };
        }
    }
}