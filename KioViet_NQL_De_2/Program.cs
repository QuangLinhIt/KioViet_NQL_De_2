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
        var bookingManager = new BookingManager(roomStatusManager);

        while (true)
        {
            // Yêu cầu người dùng nhập thông tin cho việc đặt phòng
            Console.Write("Enter RoomId: ");
            int roomId = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter start time (yyyy-MM-dd HH:mm): ");
            DateTime startTime = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter end time (yyyy-MM-dd HH:mm): ");
            DateTime endTime = DateTime.Parse(Console.ReadLine());

            // Kiểm tra việc đặt phòng
            bool result = bookingManager.BookingRoom(roomId, startTime, endTime);
            if (result)
            {
                Console.WriteLine("Booking room success.");
            }
            // Hiển thị tất cả trạng thái phòng sau khi đặt
            Console.WriteLine("\nList room status:");
            var allRoomStatuses = RoomStatusManager.GetAll(roomId);
            foreach (var status in allRoomStatuses)
            {
                Console.WriteLine($"RoomId: {status.RoomId}, Time: {status.Time}, SlotStatus: {status.SlotStatus}, IsEnableBookingByDay: {status.IsEnableBookingByDay}");
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
