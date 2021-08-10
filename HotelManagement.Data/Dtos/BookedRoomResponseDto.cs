using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Dtos
{
    public class BookedRoomResponseDto
    {
        public byte RoomNumber { get; set; }
        public bool Availability { get; set; }
        public DateTime DateBooked { get; set; }
        public byte Duration { get; set; }
        public DateTime TimeOut { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
