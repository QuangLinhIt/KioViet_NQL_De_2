using KioViet_NQL_De_2.Data;
using KioViet_NQL_De_2.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convert = KioViet_NQL_De_2.Helper.Convert;

namespace KioViet_NQL_De_2.Logic
{
    public class BookingManager
    {
        private static readonly List<Booking> _booking = new();

        /// <summary>
        /// Thêm Booking
        /// </summary>
        /// <param name="roomId">Mã phòng</param>
        /// <param name="bookingType">Kiểu booking theo giờ hoặc ngày</param>
        /// <param name="startBookingDateTime">năm-tháng-ngày giờ-phút bắt đầu đặt lịch</param>
        /// <param name="endBookingDateTime">năm-tháng-ngày giờ-phút kết thúc đặt lịch</param>
        public static void AddBooking(int roomId,string bookingType,DateTime startBookingDateTime,DateTime endBookingDateTime)
        {
            var booking = new Booking()
            {
                RoomId = roomId,
                BookingType = bookingType,
                StartBookingDateTime = startBookingDateTime,
                EndBookingDateTime = endBookingDateTime,
            };
            _booking.Add(booking);
        }

        /// <summary>
        /// hàm đặt lịch chính
        /// </summary>
        /// <param name="roomId">Mã phòng</param>
        /// <param name="startBookingDateTime">Năm-tháng-ngày giờ-phút bắt đầu đặt lịch</param>
        /// <param name="endBookingDateTime">Năm-tháng-ngày giờ-phút bắt đầu đặt lịch</param>
        /// <returns></returns>
        public static bool BookingRoom(int roomId, DateTime startBookingDateTime,DateTime endBookingDateTime)
        {
            if (endBookingDateTime <= startBookingDateTime)
            {
                Console.WriteLine("Error: endTime cannot be earlier than startTime.");
                return false;
            }

            TimeSpan duration = endBookingDateTime - startBookingDateTime;
            //TH1: khoảng thời gian lớn hơn 1 ngày -> book ngày
            if (duration.Days > 0)
            {
                var isBookingByDay= BookingByDay(roomId, DateOnly.FromDateTime(startBookingDateTime), DateOnly.FromDateTime(endBookingDateTime));
                if (isBookingByDay)
                {
                    //thêm mới Booking
                    AddBooking(roomId, "BookingByDay", startBookingDateTime, endBookingDateTime);
                    return true;
                }
                return false;
            }
            //TH2: khoảng thời gian ít hơn 1 ngày và nằm ở 2 ngày liên tiếp
            else if(DateOnly.FromDateTime(startBookingDateTime) != DateOnly.FromDateTime(endBookingDateTime))
            {
                var isBookingByHourBetweenTwoDays= BookingByHourBetweenTwoDays(roomId, startBookingDateTime, endBookingDateTime);
                if (isBookingByHourBetweenTwoDays)
                {
                    //thêm với booking
                    AddBooking(roomId, "BookingByHour", startBookingDateTime, endBookingDateTime);
                    return true;
                }
                return false;
            }
            //TH3: khoảng thời gian ít hơn 1 ngày và nằm ở cùng 1 ngày
            else
            {
                var isBookingByHour= BookingByHour(roomId, DateOnly.FromDateTime(startBookingDateTime), startBookingDateTime.Hour, endBookingDateTime.Hour);
                if (isBookingByHour)
                {
                    //thêm với booking
                    AddBooking(roomId, "BookingByHour", startBookingDateTime, endBookingDateTime);
                    return true;
                }
                return false;
            }
        }
        

        /// <summary>
        /// hàm xử lí trường hợp 1 của hàm chính BookingRoom
        /// </summary>
        /// <param name="roomId">Mã phòng</param>
        /// <param name="startDate">năm-tháng-ngày bắt đầu đặt lịch</param>
        /// <param name="endDate">năm-tháng-ngày kết thúc đặt lịch</param>
        /// <returns></returns>
        private static bool BookingByDay(int roomId,DateOnly startDate,DateOnly endDate)
        {
            //Kiểm tra tất cả các ngày xem có phù hợp
            var isEnableBookingByDate = RoomStatusManager.IsEnableBookingByDay(roomId,startDate,endDate);
            if (isEnableBookingByDate == false)
            {
                Console.WriteLine("=====> Booking by hour fail");
                return false;
            }

            //Thêm Room status
            for (DateOnly bookingDate = startDate; bookingDate < endDate; bookingDate = bookingDate.AddDays(1))
            {
                //Nếu đã cho phép đặt ngày tức là không có nên ta khởi tạo dữu liệu mặc định 
                RoomStatusManager.InitRoomStatus(roomId, bookingDate);
                var roomStatus = new RoomStatus()
                {
                    RoomId = roomId,
                    BookingDate = bookingDate,
                    BookingDateStatus = "000000000000000000000000",
                    IsEnableBookingByDay = false,
                };
                //update room status 
                RoomStatusManager.UpdateRoomStatus(roomStatus);
            }
            return true;
        }

        /// <summary>
        /// Hàm xử lí TH3 của hàm chính BookingRoom
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="bookingDate"></param>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <returns></returns>
        private static bool BookingByHour(int roomId, DateOnly bookingDate, int startHour, int endHour)
        {
            // Kiểm tra giờ có phù hợp
            var isEnableBookingByHour = RoomStatusManager.IsEnableBookingByHour(roomId, bookingDate, startHour, endHour);
            if (!isEnableBookingByHour)
            {
                Console.WriteLine("=====> Booking by hour fail");
                return false;
            }

            // Tạo array 24 phần tử kiểu bool tương ứng 24h trong ngày
            var boolSlotStatus = new bool[24] { true,true,true,true,true,true,true,true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
            //Check sự tồn tại
            var existingRoomStatus = RoomStatusManager.GetByRoomIdAndBookingDate(roomId, bookingDate);
            var roomStatus = new RoomStatus();
            roomStatus.RoomId = roomId;
            roomStatus.BookingDate = bookingDate;
            roomStatus.IsEnableBookingByDay = false;
            //nếu null tạo một data mặc định
            if (existingRoomStatus == null)
            {
                RoomStatusManager.InitRoomStatus(roomId,bookingDate);
                roomStatus.BookingDateStatus = "111111111111111111111111";
            }
            else
            {
                roomStatus.BookingDateStatus = existingRoomStatus.BookingDateStatus;
            }
            //update lại trường BookingDateStatus
            var boolBookingDateStatus = Convert.ConvertStringToBoolArray(roomStatus.BookingDateStatus);
            for (int i = startHour; i < endHour; i++)
            {
                boolBookingDateStatus[i] = false;
            }
            var stringBookingDateStatus = Convert.ConvertBoolArrayToString(boolBookingDateStatus);
            roomStatus.BookingDateStatus = stringBookingDateStatus;
            if (roomStatus.BookingDateStatus == "111111111111111111111111")
            {
                roomStatus.IsEnableBookingByDay = true;
            }
            //Update dữ liệu
            RoomStatusManager.UpdateRoomStatus(roomStatus);
            return true;
        }

        /// <summary>
        /// Hàm xứ lí TH2 của hàm chính BookingRoom
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="startBookingDateTime"></param>
        /// <param name="endBookingDateTime"></param>
        /// <returns></returns>
        private static bool BookingByHourBetweenTwoDays(int roomId,DateTime startBookingDateTime,DateTime endBookingDateTime)
        {
            // Kiểm tra giờ có phù hợp cho ngày đầu (từ giờ bắt đầu đến 24h)
            var isEnableBookingByHourInDay1 = RoomStatusManager.IsEnableBookingByHour(roomId, DateOnly.FromDateTime(startBookingDateTime), startBookingDateTime.Hour, 24);
            // Kiểm tra giờ có phù hợp cho ngày thứ hai (từ 00h00 đến giờ kết thúc)
            var isEnableBookingByHourInDay2 = RoomStatusManager.IsEnableBookingByHour(roomId, DateOnly.FromDateTime(endBookingDateTime), 0, endBookingDateTime.Hour);

            if (isEnableBookingByHourInDay1 && isEnableBookingByHourInDay2)
            {
                BookingByHour(roomId, DateOnly.FromDateTime(startBookingDateTime), startBookingDateTime.Hour,24);
                BookingByHour(roomId, DateOnly.FromDateTime(endBookingDateTime), 0, endBookingDateTime.Hour);
                return true;
            }
            return false;
        }
    }
}
