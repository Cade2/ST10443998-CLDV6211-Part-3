using ST10443998_CLDV6211_POE.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ST10443998_CLDV6211_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add this below any existing modelBuilder config:
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, TypeName = "Wedding" },
                new EventType { EventTypeId = 2, TypeName = "Birthday" },
                new EventType { EventTypeId = 3, TypeName = "Conference" },
                new EventType { EventTypeId = 4, TypeName = "Workshop" },
                new EventType { EventTypeId = 5, TypeName = "Baby Shower" }
            );
        }
    }
}
