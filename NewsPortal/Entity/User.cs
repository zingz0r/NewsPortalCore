using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Entity
{
    public class User : IdentityUser<int>
    {
        [Required]
        public override int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
