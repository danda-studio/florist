using FloristAI.Core.Entities;
using FloristAI.Core.Entities.Items;
using FloristAI.Core.Entities.ReferralsAndPartners;
using FloristAI.Core.Entities.UserInfo;
using Microsoft.EntityFrameworkCore;

namespace FloristAI.Infrastructure
{
    /// <summary>
    /// Контекст базы данных PostgreSQL для приложения FloristAI.
    /// Описывает наборы сущностей и их конфигурацию модели.
    /// </summary>
    public class PostgresDbContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста с параметрами конфигурации.
        /// </summary>
        /// <param name="options">Опции конфигурации для DbContext.</param>
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
        }

        /// <summary>Таблица пользователей.</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Таблица ролей пользователей.</summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>Таблица данных Telegram пользователей.</summary>
        public DbSet<UserTgData> UserTgDatas { get; set; }

        /// <summary>Таблица заказов.</summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>Таблица магазинов.</summary>
        public DbSet<Shop> Shops { get; set; }

        /// <summary>Таблица товаров.</summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>Таблица цветов.</summary>
        public DbSet<Flower> Flowers { get; set; }

        /// <summary>Таблица букетов.</summary>
        public DbSet<Bouquet> Bouquets { get; set; }

        /// <summary>Таблица корзин.</summary>
        public DbSet<Basket> Baskets { get; set; }

        /// <summary>Таблица партнеров.</summary>
        public DbSet<Partner> Partners { get; set; }

        /// <summary>Таблица рефералов.</summary>
        public DbSet<Referal> Referals { get; set; }

        /// <summary>Таблица связей партнеров и рефералов.</summary>
        public DbSet<PartnerReferal> PartnerReferals { get; set; }

        /// <summary>
        /// Конфигурация модели сущностей и их связей.
        /// </summary>
        /// <param name="modelBuilder">Конструктор модели для конфигурации.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация ключа для UserTgData
            modelBuilder.Entity<UserTgData>()
                .HasKey(t => t.UserId);

            // Отношение 1:1 между User и UserTgData
            modelBuilder.Entity<User>()
                .HasOne(u => u.TgData)
                .WithOne(t => t.User)
                .HasForeignKey<UserTgData>(t => t.UserId);

            // Отношение 1:1 между User и Partner
            modelBuilder.Entity<User>()
                .HasOne(u => u.Partner)
                .WithOne(p => p.User)
                .HasForeignKey<Partner>(p => p.UserId)
                .IsRequired(false);

            // Отношение 1:1 между User и Referal
            modelBuilder.Entity<User>()
                .HasOne(u => u.Referal)
                .WithOne(r => r.User)
                .HasForeignKey<Referal>(r => r.ReferalId);

            // Отношение 1:N между User и UserRole с составным ключом
            modelBuilder.Entity<UserRole>()
                .HasKey(r => new { r.UserId, r.Role });

            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(r => r.UserId);

            // Отношение 1:N между User и Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Order)
                .HasForeignKey(o => o.UserId);

            // Отношение 1:N между User и Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transaction)
                .HasForeignKey(t => t.UserId);

            // Отношение M:1 между Order и Product
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);

            // Отношение 1:1 между Product и Flower
            modelBuilder.Entity<Flower>()
                .HasOne(f => f.Product)
                .WithOne(p => p.Flower)
                .HasForeignKey<Flower>(f => f.ProductId);

            // Отношение 1:1 между Product и Basket
            modelBuilder.Entity<Basket>()
                .HasOne(b => b.Product)
                .WithOne(p => p.Basket)
                .HasForeignKey<Basket>(b => b.ProductId);

            // Отношение 1:1 между Product и Bouquet
            modelBuilder.Entity<Bouquet>()
                .HasOne(b => b.Product)
                .WithOne(p => p.Bouquet)
                .HasForeignKey<Bouquet>(b => b.ProductId);

            // Отношение 1:N между Partner и PartnerReferal с составным ключом
            modelBuilder.Entity<PartnerReferal>()
                .HasKey(pr => new { pr.PartnerId, pr.ReferalId });

            modelBuilder.Entity<PartnerReferal>()
                .HasOne(pr => pr.Partner)
                .WithMany(p => p.Partners)
                .HasForeignKey(pr => pr.PartnerId);

            // Отношение 1:1 между Referal и PartnerReferal
            modelBuilder.Entity<PartnerReferal>()
                .HasOne(pr => pr.Referal)
                .WithOne(r => r.PartnerReferal)
                .HasForeignKey<PartnerReferal>(pr => pr.ReferalId);
        }
    }
}
