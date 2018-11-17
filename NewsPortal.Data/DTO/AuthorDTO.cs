using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Data.DTO
{
    public class AuthorDTO : IdentityUser<int>
    {
        public string Name { get; set; }

    }
}
