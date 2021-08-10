using HotelManagement.Data.Models;
using HotelManagement.Data.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Services.Implementation
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelManagementDbContext _dbContext;

        public RoomRepository(HotelManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Room> GetRoomById(string id)
        {
            return await _dbContext.RoomTbl.Include(x => x.BookingHistory).FirstOrDefaultAsync(x => x.RoomId == id);
        }

        public async Task<Room> GetRoomByRoomNumber(byte roomNumber)
        {
            return await _dbContext.RoomTbl.FirstOrDefaultAsync(x => x.RoomNumber == roomNumber);
        }

        public async Task<IEnumerable<Room>> GetRooms()
        {
            var rooms = await _dbContext.RoomTbl.ToListAsync();
            return rooms.ToList();
        }

        public async Task<bool> IsRoomAvailable(byte roomNumber)
        {
            Room checkRoom = await _dbContext.RoomTbl.FirstOrDefaultAsync(x => x.RoomNumber == roomNumber);
            return checkRoom.Availability == true;
        }

        public async Task<bool> IsRoomBooked(string roomId)
        {
            Room checkRoom = await _dbContext.RoomTbl.FirstOrDefaultAsync(x => x.RoomId == roomId);
            return checkRoom.Availability == true;
        }

        public async Task<IEnumerable<Room>> GetAvailableRooms()
        {
            var availableRooms = await _dbContext.RoomTbl.Where(x => x.Availability == true).ToListAsync();
            return availableRooms;
        }

        public async Task<IEnumerable<Room>> GetBookedRooms()
        {
            var availableRooms = await _dbContext.RoomTbl.Where(x => x.Availability == false).ToListAsync();
            return availableRooms;
        }

        public async Task<IEnumerable<Room>> GetOverdueRooms()
        {
            var overdueRooms = await _dbContext.RoomTbl.Include(x => x.BookingHistory).Where(x => x.Availability == false && x.BookingHistory.OrderBy(x => x.DateBooked).LastOrDefault().TimeOut < DateTime.Now).ToListAsync();
            return overdueRooms;
        }

        //public async Task<IEnumerable<Room>> GetAnOverdueRoomById(string roomId)
        //{
        //    var overdueRoom = await _dbContext.RoomTbl.Include(x => x.BookingHistory).Where(x => x.Availability == false && x.BookingHistory.OrderBy(x => x.DateBooked).LastOrDefault().TimeOut < DateTime.Now).Where(x => x.RoomId == roomId).ToListAsync();
        //    return overdueRoom;
        //}

        public async Task<bool> UpdateRoomPhoto(Room room)
        {
            _dbContext.RoomTbl.Update(room);

            return await _dbContext.SaveChangesAsync() > 0;
        }

    }
}
