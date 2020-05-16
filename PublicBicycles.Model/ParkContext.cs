using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PublicBicycles.Models
{
    public class PublicBicyclesContext : DbContext
    {
        public PublicBicyclesContext(DbContextOptions<PublicBicyclesContext> options) : base(options)
        {
        }
        public PublicBicyclesContext() : base()
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarOwner> CarOwners { get; set; }
        public DbSet<PublicBicyclesArea> PublicBicyclesAreas { get; set; }
        public DbSet<PublicBicyclesingSpace> PublicBicyclesingSpaces { get; set; }
        public DbSet<Config> Aisles { get; set; }
        public DbSet<Wall> Walls { get; set; }
        public DbSet<PublicBicyclesRecord> PublicBicyclesRecords { get; set; }
        public DbSet<TransactionRecord> TransactionRecords { get; set; }
        public DbSet<PriceStrategy> PriceStrategys { get; set; }
        public DbSet<Config> Configs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //车与车主
            modelBuilder.Entity<Car>()
                .HasOne(c => c.CarOwner)
                .WithMany(o => o.Cars)
                .HasForeignKey(c => c.CarOwnerID)
                .OnDelete(DeleteBehavior.Cascade);

            //车与停车记录
            modelBuilder.Entity<PublicBicyclesRecord>()
                .HasOne(p => p.Car)
                .WithMany(c => c.PublicBicyclesRecords)
                .HasForeignKey(p => p.CarID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //停车区和停车策略
            modelBuilder.Entity<PublicBicyclesArea>()
                .HasOne(a => a.PriceStrategy)
                .WithMany()
                .HasForeignKey(a => a.PriceStrategyID)
                .OnDelete(DeleteBehavior.Cascade);

            //停车位和停车区
            modelBuilder.Entity<PublicBicyclesingSpace>()
                .HasOne(s => s.PublicBicyclesArea)
                .WithMany(a => a.PublicBicyclesingSpaces)
                .HasForeignKey(s => s.PublicBicyclesAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            //停车区和过道
            modelBuilder.Entity<Aisle>()
                .HasOne(a => a.PublicBicyclesArea)
                .WithMany(a => a.Aisles)
                .HasForeignKey(s => s.PublicBicyclesAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            //停车区和墙
            modelBuilder.Entity<Wall>()
                .HasOne(a => a.PublicBicyclesArea)
                .WithMany(p => p.Walls)
                .HasForeignKey(s => s.PublicBicyclesAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //车主和交易记录
            modelBuilder.Entity<TransactionRecord>()
                .HasOne(t => t.CarOwner)
                .WithMany(o => o.TransactionRecords)
                .HasForeignKey(t => t.CarOwnerID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //停车区和停车记录
            modelBuilder.Entity<PublicBicyclesRecord>()
                .HasOne(r => r.PublicBicyclesArea)
                .WithMany()
                .HasForeignKey(r => r.PublicBicyclesAreaID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }

    }

}
