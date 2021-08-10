using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Models
{
    public class Room
    {
        public Room()
        {
            BookingHistory = new HashSet<BookingHistory>();
        }
        public string RoomId { get; set; } = Guid.NewGuid().ToString();
        public byte RoomNumber { get; set; }
        public bool Availability { get; set; }
        public string RoomPhotoUrl { get; set; }
        public IEnumerable<BookingHistory> BookingHistory { get; set; }
    }
}
