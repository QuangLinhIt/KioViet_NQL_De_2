using KioViet_NQL_De_2.Data;
using KioViet_NQL_De_2.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Convert = KioViet_NQL_De_2.Helper.Convert;

namespace KioViet_NQL_De_2.Logic
{
    public class RoomStatusManager
    {
        private static readonly List<RoomStatus> _roomStatus = new();
        /// <summary>
        /// hàm lấy thông tin RoomStatus theo RoomId và BookingDate
        /// </summary>
        /// <param name="roomId">xác định mã phòng khách sạn</param>
        /// <param name="bookingDate">xác định ngày-tháng-năm đặt lịch</param>
        /// <returns></returns>
        public static RoomStatus? GetByRoomIdAndBookingDate(int roomId, DateOnly bookingDate)
        {
            return _roomStatus.FirstOrDefault(x =>x.RoomId==roomId && x.BookingDate == bookingDate);
        }

        /// <summary>
        /// Lấy danh sách RoomStatus theo RoomId.
        /// </summary>
        /// <param name="roomId">Mã phòng khách sạn</param>
        /// <returns>Danh sách RoomStatus của phòng</returns>
        public static List<RoomStatus> GetByRoomId(int roomId)
        {
            return _roomStatus.Where(x => x.RoomId == roomId).ToList();
        }

        /// <summary>
        /// Khởi tạo 1 RoomStatus có thể đặt lịch theo ngày hoặc bất cứ giờ nào trong 24h
        /// </summary>
        /// <param name="roomId">xác định mã phòng khách sạn</param>
        /// <param name="bookingDate">xác định ngày-tháng-năm đặt lịch</param>
        public static void InitRoomStatus(int roomId,DateOnly bookingDate)
        {
            var roomStatus = new RoomStatus()
            {
                RoomId = roomId,
                BookingDate = bookingDate,
                BookingDateStatus = "111111111111111111111111",
                IsEnableBookingByDay=true,
            };
            _roomStatus.Add(roomStatus);
        }

        /// <summary>
        /// Hàm cập nhật dữ liệu RoomStatus sau khi đặt lịch thành công
        /// </summary>
        /// <param name="roomStatus">Model RoomStatus</param>
        /// <returns></returns>
        public static bool UpdateRoomStatus(RoomStatus roomStatus)
        {
            var existingStatus = _roomStatus.FirstOrDefault(x =>x.RoomId== roomStatus.RoomId && x.BookingDate == roomStatus.BookingDate);
            if (existingStatus != null)
            {
                existingStatus.BookingDateStatus = roomStatus.BookingDateStatus;
                existingStatus.IsEnableBookingByDay = roomStatus.IsEnableBookingByDay;
                return true; // Trả về true nếu cập nhật thành công
            }
            return false;
        }


        /// <summary>
        /// Hàm check xem mã phòng còn trống từ ngày này tới ngày kia không
        /// </summary>
        /// <param name="roomId">xác định mã phòng khách sạn</param>
        /// <param name="startDate">Ngày-tháng-năm bắt đầu đặt lịch</param>
        /// <param name="endDate">Ngày-tháng-năm kết thúc đặt lịch</param>
        /// <returns></returns>
        public static bool IsEnableBookingByDay(int roomId,DateOnly startDate,DateOnly endDate)
        {
            for(var bookingDate = startDate; bookingDate < endDate; bookingDate = bookingDate.AddDays(1))
            {
                var existingRoomStatus = _roomStatus.FirstOrDefault(x =>x.RoomId==roomId && x.BookingDate == bookingDate);
                if(existingRoomStatus!=null && existingRoomStatus.IsEnableBookingByDay == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra xem mã phòng ở ngày-tháng-năm này có trống tại giờ đặt k
        /// </summary>
        /// <param name="roomId">xác định mã phòng khách sạn</param>
        /// <param name="bookingDate">Ngày-tháng-năm đặt lịch</param>
        /// <param name="startHour">Giờ bắt đầu</param>
        /// <param name="endHour">Giờ kết thúc</param>
        /// <returns></returns>
        public static bool IsEnableBookingByHour(int roomId,DateOnly bookingDate,int startHour,int endHour)
        {
            
            var existingRoomStatus = _roomStatus.FirstOrDefault(x => x.BookingDate == bookingDate);
            if(existingRoomStatus==null)
            {
                return true;
            }
            var boolSlotStatus = Convert.ConvertStringToBoolArray(existingRoomStatus.BookingDateStatus);
            for(int i = startHour; i < endHour; i++)
            {
                if (boolSlotStatus[i] == false)
                {
                    return false;
                }
            }
            return true;
        }
       
    }
}
