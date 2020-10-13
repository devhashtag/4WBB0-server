using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PocketServer.DataAccess.Entities;

namespace PocketServer.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertResponse> AlertResponses { get; set; }
        public DbSet<Heartbeat> Heartbeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure constraints
            modelBuilder.Entity<Subscription>()
                .HasKey(s => new { s.UserId, s.DeviceId });
        }
    }
}
