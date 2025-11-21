using ClaimSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClaimSystem.Data
{
    public class ClaimSystemContext : IdentityDbContext<ApplicationUser>
    {
        public ClaimSystemContext(DbContextOptions<ClaimSystemContext> options) : base(options) { }

        public DbSet<ClaimRecord> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ClaimRecord>()
                .HasKey(c => c.ClaimId);

            builder.Entity<ApplicationUser>()
                .HasMany<ClaimRecord>()
                .WithOne(c => c.Lecturer)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
