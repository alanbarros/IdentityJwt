using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityJwt.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid UserId { get; set; }
    }
}