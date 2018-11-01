using System.ComponentModel.DataAnnotations;

namespace NewsPortal.Entity
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
