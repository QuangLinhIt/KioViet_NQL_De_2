using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioViet_NQL_De_2.Data
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BookingType { get; set; }
        public DateTime StartBookingDateTime { get; set; }
        public DateTime EndBookingDateTime { get; set; }
    }
}
