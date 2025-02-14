using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HousingSManagement.Models;  // Ensure correct namespace

namespace HousingSManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // ✅ Uses ApplicationUser
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define Database Tables
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<MaintenancePayment> Payments { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<House> Houses { get; set; } // ✅ Ensure House table is included

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Ensure Decimal Precision for `MaintenancePayment.Amount`
            builder.Entity<MaintenancePayment>()
                .Property(m => m.Amount)
                .HasColumnType("decimal(18,2)");

            // ✅ Role Seeding: Only Insert If Role Doesn't Exist
            var adminRoleId = "a1b2c3d4-e5f6-7890-1234-56789abcdef0";
            var residentRoleId = "b2c3d4e5-f678-9012-3456-789abcdef012";
            var securityRoleId = "c3d4e5f6-7890-1234-5678-9abcdef01234";

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = residentRoleId, Name = "Resident", NormalizedName = "RESIDENT" },
                new IdentityRole { Id = securityRoleId, Name = "Security", NormalizedName = "SECURITY" }
            };

            // ✅ Use `HasData()` Only If Roles Are Not Already Present
            builder.Entity<IdentityRole>().HasData(roles);
        }

    }
}
