using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Data.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            BookingHistory = new HashSet<BookingHistory>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<BookingHistory> BookingHistory { get; set; }
    }
}
