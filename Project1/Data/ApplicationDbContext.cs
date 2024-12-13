using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);
               
            }

        public DbSet<IdentityUser> ApplicationUsers { get; set; } 

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
