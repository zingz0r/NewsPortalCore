using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Data.Entity
{
    public class User : IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
    }
}