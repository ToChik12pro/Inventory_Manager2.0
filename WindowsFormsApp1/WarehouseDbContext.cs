using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WindowsFormsApp1
{
      internal class WarehouseDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockItem> StockItems => Set<StockItem>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public WarehouseDbContext() => Database.EnsureCreated();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
    modelBuilder.Entity<UserRole>()
        .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Конфигурация для PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>()
                .HasKey(po => po.Id);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.OrderDate)
                .IsRequired();

           
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.OrderItems)
                .WithOne(oi => oi.PurchaseOrder)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурация для PurchaseOrderItem
            modelBuilder.Entity<PurchaseOrderItem>()
                .HasKey(oi => oi.Id);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(oi => oi.PurchaseOrder)
                .WithMany(po => po.OrderItems)
                .HasForeignKey(oi => oi.PurchaseOrderId);


            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(oi => oi.Quantity)
                .IsRequired();

            // Конфигурация для Product
            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasMany(p => p.StockItems)
                .WithOne(si => si.Product)
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурация для StockItem
            modelBuilder.Entity<StockItem>()
                .HasKey(si => si.Id);

            modelBuilder.Entity<StockItem>()
                .HasOne(si => si.Product)
                .WithMany(p => p.StockItems)
                .HasForeignKey(si => si.ProductId);

            modelBuilder.Entity<StockItem>()
                .Property(si => si.Quantity)
                .IsRequired();

            modelBuilder.Entity<StockItem>()
                .Property(si => si.DateAdded)
                .IsRequired();

            // Конфигурация для Supplier
            modelBuilder.Entity<Supplier>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Supplier>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Supplier>()
                .Property(s => s.ContactPerson)
                .IsRequired();

            // Конфигурация для User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурация для Role
            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Замените строку подключения на вашу конфигурацию базы данных
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-56IS6DV\SQL;Database=warehousedb;Integrated Security=True;TrustServerCertificate=True");
            

        }
    }
}
