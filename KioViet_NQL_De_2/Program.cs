using KioViet_NQL_De_2.Data;
using KioViet_NQL_De_2.Logic;
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Khởi tạo các manager
        var roomStatusManager = new RoomStatusManager();
        var bookingManager = new BookingManager();

        while (true)
        {
            Console.WriteLine("_________________________________________________________________________________________");
            // Yêu cầu người dùng nhập thông tin cho việc đặt phòng
            Console.Write("Enter RoomId: ");
            int roomId = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter start time (yyyy-MM-dd HH): ");
            string startTimeInput = Console.ReadLine();
            DateTime startTime = DateTime.Parse($"{startTimeInput}:00"); // Thêm phút = 00

            Console.Write("Enter end time (yyyy-MM-dd HH): ");
            string endTimeInput = Console.ReadLine();
            DateTime endTime = DateTime.Parse($"{endTimeInput}:00"); // Thêm phút = 00


            // Kiểm tra việc đặt phòng
            bool result = BookingManager.BookingRoom(roomId, startTime, endTime);
            if (result)
            {
                Console.WriteLine("=====> Booking room success.");
            }
            // Hiển thị tất cả trạng thái phòng sau khi đặt
            Console.WriteLine("\nList room status:");
            var allRoomStatuses = RoomStatusManager.GetByRoomId(roomId);
            foreach (var item in allRoomStatuses)
            {
                Console.WriteLine($"RoomId: {item.RoomId}, BookingDate: {item.BookingDate}, BookingDateStatus: {item.BookingDateStatus}, IsEnableBookingByDay: {item.IsEnableBookingByDay}");
            }

            // Hỏi người dùng có muốn tiếp tục không
            Console.Write("\ndo you want to continue ? (y/n): ");
            string continueBooking = Console.ReadLine();

            if (continueBooking.ToLower() != "y")
            {
                break;
            }
        }

        Console.WriteLine("Goodbye");
    }
}
