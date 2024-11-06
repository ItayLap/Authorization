using Authorization.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Data
{
    public class ApplicationDbContext   :   IdentityDbContext<ApplicationUserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUserModel>()
                .Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<ApplicationUserModel>()
                .Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
