using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HeliosBot.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PapertradeUser> PapertradeUser { get; set; }
        public DbSet<PapertradeOwnedStock> PapertradeOwnedStock { get; set; }
        public DbSet<PapertradeTransaction> PapertradeTransaction { get; set; }

        private readonly string _connectionString;

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PapertradeUser>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Money).IsRequired();
                entity.HasMany(e => e.OwnedStocks)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId);
                entity.HasMany(e => e.Transactions)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId);
            });

            modelBuilder.Entity<PapertradeOwnedStock>(entity =>
            {
                entity.HasKey(e => e.OwnedStockId);
                entity.Property(e => e.StockCode).IsRequired();
                entity.Property(e => e.Shares).IsRequired();
                entity.HasOne(d => d.User)
                    .WithMany(p => p.OwnedStocks)
                    .HasForeignKey(p => p.UserId);
            });

            modelBuilder.Entity<PapertradeTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.StockCode).IsRequired();
                entity.Property(e => e.Shares).IsRequired();
                entity.Property(e => e.TransactionType).IsRequired();
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(p => p.UserId);
            });
        }
    }
}
