using HotelManagement.Data.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data
{
    public class HotelManagementDbContext : IdentityDbContext<AppUser>
    {
        private readonly DbContextOptions<HotelManagementDbContext> _options;

        public HotelManagementDbContext(DbContextOptions<HotelManagementDbContext> options) : base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Room>().HasIndex(x => x.RoomNumber);
            builder.Entity<Room>().HasIndex(x => x.Availability);
            base.OnModelCreating(builder);
        }

        public DbSet<Room> RoomTbl { get; set; }
        public DbSet<BookingHistory> BookingHistoryTbl { get; set; }
    }
}
