using HotelManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Services.Interface
{
    public interface IRoomRepository
    {
        public Task<IEnumerable<Room>> GetRooms();
        public Task<Room> GetRoomById(string id);
        public Task<Room> GetRoomByRoomNumber(byte roomNumber);
        public Task<bool> IsRoomAvailable(byte roomNumber);
        public Task<bool> IsRoomBooked(string roomId);
        public Task<IEnumerable<Room>> GetAvailableRooms();
        public Task<IEnumerable<Room>> GetBookedRooms();
        public Task<IEnumerable<Room>> GetOverdueRooms();
        public Task<bool> UpdateRoomPhoto(Room room);
    }
}
