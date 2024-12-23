using KioViet_NQL_De_2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioViet_NQL_De_2.Logic
{
    public class BookingManager
    {
        private readonly RoomStatusManager _roomStatusManager;

        public BookingManager(RoomStatusManager roomStatusManager)
        {
            _roomStatusManager = roomStatusManager;
        }
        //đặt phòng
        public bool BookingRoom(int roomId, DateTime startTime,DateTime endTime)
        {
            if (endTime < startTime)
            {
                Console.WriteLine("Error: endTime cannot be earlier than startTime.");
                return false;
            }

            TimeSpan duration = endTime - startTime;
            //TH1: khoảng thời gian lớn hơn 1 ngày -> book ngày
            if (duration.Days > 0)
            {
                return BookingByDay(roomId, DateOnly.FromDateTime(startTime), DateOnly.FromDateTime(endTime));
            }
            //TH2: khoảng thời gian ít hơn 1 ngày và nằm ở 2 ngày liên tiếp
            else if(DateOnly.FromDateTime(startTime) != DateOnly.FromDateTime(endTime))
            {
                return BookingByHourBetweenTwoDays(roomId, startTime, endTime);

            }
            //TH3: khoảng thời gian ít hơn 1 ngày và nằm ở cùng 1 ngày
            else if (duration.Hours > 0)
            {
                return BookingByHour(roomId, DateOnly.FromDateTime(startTime), startTime.Hour, endTime.Hour);
            }
            else
            {
                return false;
            }
        }
        //Đặt phòng theo ngày
        private bool BookingByDay(int roomId,DateOnly startDay,DateOnly endDay)
        {
            //Kiểm tra tất cả các ngày xem có phù hợp
            for(DateOnly time = startDay; time <= endDay; time = time.AddDays(1))
            {
                var isEnableBookingByDate = _roomStatusManager.IsEnableBookingByDay(roomId, time);
                if (isEnableBookingByDate == false)
                {
                    Console.WriteLine("Booking by day fail");
                    return false;
                }
            }
            //Thêm hoặc chỉnh sửa Room status
            for (DateOnly time = startDay; time <= endDay; time = time.AddDays(1))
            {
                var existingRoomStatus = RoomStatusManager.GetRoomStatus(roomId, time);
                var roomStatus = new RoomStatus()
                {
                    RoomId = roomId,
                    Time = time,
                    SlotStatus = "000000000000000000000000",
                    IsEnableBookingByDay = false,
                };
                if (existingRoomStatus == null)
                {
                    RoomStatusManager.AddRoomStatus(roomStatus);
                }
                RoomStatusManager.UpdateRoomStatus(roomStatus);
            }
            return true;
        }

        // Đặt phòng theo giờ trong 1 ngày
        private bool BookingByHour(int roomId, DateOnly time, int startHour, int endHour)
        {
            // Kiểm tra giờ có phù hợp
            var isEnableBookingByHour = _roomStatusManager.IsEnableBookingByHour(roomId, time, startHour, endHour);
            if (!isEnableBookingByHour)
            {
                Console.WriteLine("Booking by hour fail");
                return false;
            }

            // Tạo array 24 phần tử kiểu bool tương ứng 24h trong ngày
            var boolSlotStatus = new bool[24];
            // Mặc định tất cả bằng true
            for (int i = 0; i < 24; i++)
            {
                boolSlotStatus[i] = true;
            }
            // Đặt các giờ đã chọn thành false
            for (int i = startHour; i <= endHour; i++)
            {
                boolSlotStatus[i] = false;
            }

            var convert = new KioViet_NQL_De_2.Helper.Convert();
            var stringSlotStatus = convert.ConvertBoolArrayToString(boolSlotStatus);
            var roomStatus = new RoomStatus()
            {
                RoomId = roomId,
                Time = time,
                SlotStatus = stringSlotStatus,
                IsEnableBookingByDay = false,
            };

            // Thêm hoặc chỉnh sửa Room Status
            var existingRoomStatus = RoomStatusManager.GetRoomStatus(roomId, time);
            if (existingRoomStatus == null)
            {
                RoomStatusManager.AddRoomStatus(roomStatus);
            }
            else
            {
                RoomStatusManager.UpdateRoomStatus(roomStatus);
            }
            return true;
        }

        //Đặt phòng theo giờ (<24h) nhưng khác ngày (nằm giữa 2 ngày liên tiếp)
        private bool BookingByHourBetweenTwoDays(int roomId,DateTime startTime,DateTime endTime)
        {
            // Kiểm tra giờ có phù hợp cho ngày đầu (từ giờ bắt đầu đến 23h59)
            var isEnableBookingByHourInDay1 = _roomStatusManager.IsEnableBookingByHour(roomId, DateOnly.FromDateTime(startTime), startTime.Hour, 23);
            // Kiểm tra giờ có phù hợp cho ngày thứ hai (từ 00h00 đến giờ kết thúc)
            var isEnableBookingByHourInDay2 = _roomStatusManager.IsEnableBookingByHour(roomId, DateOnly.FromDateTime(endTime), 0, endTime.Hour);

            if (isEnableBookingByHourInDay1 && isEnableBookingByHourInDay2)
            {
                BookingByHour(roomId, DateOnly.FromDateTime(startTime),startTime.Hour,23);
                BookingByHour(roomId, DateOnly.FromDateTime(endTime), 0, endTime.Hour);
                return true;
            }
            return false;
        }
    }
}
