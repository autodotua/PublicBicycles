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
        /// <summary>
        /// 自行车
        /// </summary>
        public DbSet<Bicycle> Bicycles { get; set; }
        /// <summary>
        /// 借车记录
        /// </summary>
        public DbSet<Hire> Hires { get; set; }
        /// <summary>
        /// 租赁点
        /// </summary>
        public DbSet<Station> Stations { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// 配置（无用）
        /// </summary>
        public DbSet<Config> Configs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Bicycle>()
            //     .HasOne(p => p.Station)
            //     .WithMany(p => p.Bicycles)
            //     .HasForeignKey(p => p.StationID);
            //modelBuilder.Entity<Hire>()
            //     .HasOne(p => p.Hirer)
            //     .WithMany()
            //     .HasForeignKey(p => p.HirerID);
            //modelBuilder.Entity<Hire>()
            //     .HasOne(p => p.HireStation)
            //     .WithMany()
            //     .HasForeignKey(p => p.HireStationID);
            //modelBuilder.Entity<Hire>()
            //     .HasOne(p => p.ReturnStation)
            //     .WithMany()
            //     .HasForeignKey(p => p.ReturnStationID);
        }

    }

}
