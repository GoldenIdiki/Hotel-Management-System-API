using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Dtos
{
    public class RoomResponseDto
    {
        public byte RoomNumber { get; set; }
        public bool Availability { get; set; }
    }
}
