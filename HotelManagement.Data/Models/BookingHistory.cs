using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Models
{
    public class BookingHistory
    {
        public BookingHistory()
        {
            BookingHistoryId = Guid.NewGuid().ToString();
            DateBooked = DateTime.Now;
        }
        public string BookingHistoryId { get; set; }
        public DateTime DateBooked { get; set; }
        public byte Duration { get; set; }
        public DateTime TimeOut { get; set; }
        public byte RoomNumber { get; set; }

        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        public string RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }
    }
}
