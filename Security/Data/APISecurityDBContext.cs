using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityJwt.Security.Data
{
    public class APISecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public APISecurityDbContext(DbContextOptions<APISecurityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}