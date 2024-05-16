using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineConsultationPlatform.Data.Entities;

namespace OnlineConsultationPlatform.Data.DbContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<Report> Reports { get; set; }
    }
}
