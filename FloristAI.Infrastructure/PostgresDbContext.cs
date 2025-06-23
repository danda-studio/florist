using FloristAI.Core.Entities;
using FloristAI.Core.Entities.Items;
using FloristAI.Core.Entities.ReferralsAndPartners;
using FloristAI.Core.Entities.UserInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure
{
    public class PostgresDbContext : DbContext
    {

        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserTgData> UserTgDatas { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Bouquet> Bouquets { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Referal> Referals { get; set; }
        public DbSet<PartnerReferal> PartnerReferals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserTgData>()
                .HasKey(t => t.UserId);

            // User - UserTgData (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.TgData)
                .WithOne(t => t.User)
                .HasForeignKey<UserTgData>(t => t.UserId);

            // User - Partner (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Partner)
                .WithOne(p => p.User)
                .HasForeignKey<Partner>(p => p.UserId);

            // User - Referal (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Referal)
                .WithOne(r => r.User)
                .HasForeignKey<Referal>(r => r.ReferalId);

            //  User - UserRole (1:N)
            modelBuilder.Entity<UserRole>()
                .HasKey(r => new { r.UserId, r.Role });

            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(r => r.UserId);

            // User - Order (1:N)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Order)
                .HasForeignKey(o => o.UserId);

            // User - Transaction (1:N)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transaction)
                .HasForeignKey(t => t.UserId);

            // Order - Product (M:1)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);

            // Product - Flower/Basket/Bouquet (1:1)
            modelBuilder.Entity<Flower>()
                .HasOne(f => f.Product)
                .WithOne(p => p.Flower)
                .HasForeignKey<Flower>(f => f.ProductId);

            modelBuilder.Entity<Basket>()
                .HasOne(b => b.Product)
                .WithOne(p => p.Basket)
                .HasForeignKey<Basket>(b => b.ProductId);

            modelBuilder.Entity<Bouquet>()
                .HasOne(b => b.Product)
                .WithOne(p => p.Bouquet)
                .HasForeignKey<Bouquet>(b => b.ProductId);

            // Partner - PartnerReferal (1:N)
            modelBuilder.Entity<PartnerReferal>()
                .HasKey(pr => new { pr.PartnerId, pr.ReferalId });

            modelBuilder.Entity<PartnerReferal>()
                .HasOne(pr => pr.Partner)
                .WithMany(p => p.Partners)
                .HasForeignKey(pr => pr.PartnerId);

            modelBuilder.Entity<PartnerReferal>()
                .HasOne(pr => pr.Referal)
                .WithOne(r => r.PartnerReferal)
                .HasForeignKey<PartnerReferal>(pr => pr.ReferalId);
        }
    }
}
