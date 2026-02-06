using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using VendorManager.Data.Models;

namespace VendorManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=VendorManagerConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<IPMac> IPMacs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vendor>()
                .Property(v => v.macs)
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Vendor>()
                .HasIndex(v => v.macs)
                .IsUnique();

            modelBuilder.Entity<IPMac>()
                .Property(i => i.Mac)
                .IsRequired()
                .HasMaxLength(17);

            modelBuilder.Entity<IPMac>()
                .Property(i => i.ip_cur)
                .HasMaxLength(20);
        }
    }
}