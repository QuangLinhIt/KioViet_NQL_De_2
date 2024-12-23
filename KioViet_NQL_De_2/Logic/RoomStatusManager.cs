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
        private static readonly ConcurrentDictionary<int, List<RoomStatus>> _roomStatuses = new();

        public static RoomStatus? GetRoomStatus(int roomId, DateOnly time)
        {
            if (_roomStatuses.TryGetValue(roomId, out var roomStatusesList))
            {
                return roomStatusesList.FirstOrDefault(x => x.Time == time);
            }
            return null;
        }

        public static void AddRoomStatus(RoomStatus status)
        {
            // Nếu RoomId không tồn tại, tạo mới danh sách
            if (!_roomStatuses.ContainsKey(status.RoomId))
            {
                _roomStatuses[status.RoomId] = new List<RoomStatus>();
            }
            // Thêm trạng thái mới
            _roomStatuses[status.RoomId].Add(status);
        }

        public static bool UpdateRoomStatus(RoomStatus status)
        {
            if (_roomStatuses.TryGetValue(status.RoomId, out var roomStatusesList))
            {
                var existingStatus = roomStatusesList.FirstOrDefault(x => x.Time == status.Time);
                if (existingStatus != null)
                {
                    existingStatus.SlotStatus = status.SlotStatus;
                    existingStatus.IsEnableBookingByDay = status.IsEnableBookingByDay;
                    return true; // Trả về true nếu cập nhật thành công
                }
            }
            return false; // Trả về false nếu không tìm thấy để cập nhật
        }

        public static List<RoomStatus> GetAll(int roomId)
        {
            if (_roomStatuses.TryGetValue(roomId, out var roomStatusesList))
            {
                return roomStatusesList
                    .OrderBy(status => status.Time)   // Sort by Time
                    .ThenBy(status => status.RoomId)  // Then sort by RoomId
                    .ToList();  // Convert to a list and return
            }
            return new List<RoomStatus>(); // Return an empty list if no room statuses are found
        }

        public bool IsEnableBookingByDay(int roomId,DateOnly time)
        {
            if(_roomStatuses.TryGetValue(roomId,out var roomStatusesList))
            {
                var existingRoomStatus = roomStatusesList.FirstOrDefault(x => x.Time == time);
                if(existingRoomStatus==null)
                {
                    return true;
                }
                if (existingRoomStatus.IsEnableBookingByDay == true)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public bool IsEnableBookingByHour(int roomId,DateOnly time,int startHour,int endHour)
        {
            if(_roomStatuses.TryGetValue(roomId,out var roomStatusesList))
            {
                var existingRoomStatus = roomStatusesList.FirstOrDefault(x => x.Time == time);
                if(existingRoomStatus==null)
                {
                    return true;
                }
                var convert = new KioViet_NQL_De_2.Helper.Convert();
                var boolSlotStatus = convert.ConvertReversedStringToBoolArray(existingRoomStatus.SlotStatus);
                for(int i = startHour; i <= endHour; i++)
                {
                    if (boolSlotStatus[i] == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
       
    }
}
