using KioViet_NQL_De_2.Data;
using KioViet_NQL_De_2.Logic;
using System;

class Program
{
    static void Main(string[] args)
    {
        var roomStatusManager = new RoomStatusManager();
        var bookingManager = new BookingManager();

        while (true)
        {
            Console.WriteLine("_________________________________________________________________________________________");

            int roomId = GetValidInteger("Enter RoomId: ");

            // Yêu cầu người dùng chọn kiểu đặt phòng
            Console.Write("Choose booking type (1: Day, 2: Hour): ");
            int bookingType = GetValidInteger("Enter 1 or 2: ", new[] { 1, 2 });

            DateTime startTime;
            DateTime endTime;

            if (bookingType == 1) // Đặt theo ngày
            {
                startTime = GetValidDate("Enter start date (yyyy-MM-dd): ");
                endTime = GetValidDate("Enter end date (yyyy-MM-dd): ").AddDays(1).AddSeconds(-1); // Cuối ngày
            }
            else // Đặt theo giờ
            {
                startTime = GetValidDateTime("Enter start datetime (yyyy-MM-dd HH): ");
                endTime = GetValidDateTime("Enter end datetime (yyyy-MM-dd HH): ");
            }

            // Kiểm tra việc đặt phòng
            bool result = BookingManager.BookingRoom(roomId, startTime, endTime);
            if (result)
            {
                Console.WriteLine("=====> Booking room success.");
            }

            // Hiển thị trạng thái phòng
            Console.WriteLine("\nList room status:");
            var allRoomStatuses = RoomStatusManager.GetByRoomId(roomId);
            foreach (var item in allRoomStatuses)
            {
                Console.WriteLine($"RoomId: {item.RoomId}, BookingDate: {item.BookingDate}, BookingDateStatus: {item.BookingDateStatus}, IsEnableBookingByDay: {item.IsEnableBookingByDay}");
            }

            // Hỏi tiếp tục
            Console.Write("\nDo you want to continue? (y/n): ");
            string continueBooking = Console.ReadLine();

            if (continueBooking.ToLower() != "y")
            {
                break;
            }
        }

        Console.WriteLine("Goodbye");
    }

    static int GetValidInteger(string prompt, int[] validOptions = null)
    {
        int value;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value) && (validOptions == null || validOptions.Contains(value)))
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
    }

    static DateTime GetValidDate(string prompt)
    {
        DateTime value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (DateTime.TryParse(input, out value) && value.TimeOfDay == TimeSpan.Zero) // Chỉ ngày
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please enter a valid date in the format yyyy-MM-dd.");
        }
    }

    static DateTime GetValidDateTime(string prompt)
    {
        DateTime value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (DateTime.TryParse($"{input}:00", out value)) // Ngày và giờ
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please enter a valid datetime in the format yyyy-MM-dd HH.");
        }
    }
}
