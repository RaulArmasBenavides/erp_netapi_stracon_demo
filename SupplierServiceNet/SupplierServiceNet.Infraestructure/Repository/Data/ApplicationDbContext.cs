using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupplierServiceNet.Core.Entities;

namespace SupplierServiceNet.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurar la relación para incluir Role
            builder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);

            // Global query filters for soft delete
            builder.Entity<Supplier>()
                .HasQueryFilter(s => !s.IsDeleted);

            builder.Entity<PurchaseRequest>()
                .HasQueryFilter(pr => !pr.IsDeleted);

            builder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);
        }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<User> Users { get; set; }
    }
 
}