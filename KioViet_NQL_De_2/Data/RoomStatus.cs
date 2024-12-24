using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioViet_NQL_De_2.Data
{
    public class RoomStatus
    {
        public int RoomId { get; set; }
        public DateOnly BookingDate { get; set; }

        public string BookingDateStatus { get; set; }

        //trường IsAvailable sẽ bằng false nếu có bất cứ 1 giờ trong ngày nào được booking
        public bool IsEnableBookingByDay { get; set; }
    }
}
