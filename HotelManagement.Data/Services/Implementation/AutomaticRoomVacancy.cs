using HotelManagement.Data.Models;
using HotelManagement.Data.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Services.Implementation
{
    public class AutomaticRoomVacancy : IAutomaticRoomVacancy
    {
        private readonly IRoomRepository _roomRepository;
        private readonly HotelManagementDbContext _dbContext;

        public AutomaticRoomVacancy(IRoomRepository roomRepository, HotelManagementDbContext dbContext)
        {
            _roomRepository = roomRepository;
            _dbContext = dbContext;
        }
        public async Task<bool> MakeRoomVacant()
        {
            var overdueRooms = await _roomRepository.GetOverdueRooms();
            foreach (Room room in overdueRooms)
            {
                room.Availability = true;
            }

            _dbContext.UpdateRange(overdueRooms);
            var result = await _dbContext.SaveChangesAsync();
            return result >= 1;
        }
    }
}
