Đề bài 2:
Khách sạn X có N phòng.
Để quản lý việc đặt phòng và check phòng trống thì cần lưu dữ liệu đặt phòng như thế nào tối ưu nhất để:
1. Có thể check phòng trống theo ngày
2. Có thể check phòng trống theo giờ trong ngày.

Ví dụ: Ngày 1/1/2024 đã có đặt phòng A vào lúc 13h-15h.
1. Làm sao để check nhanh nhất phòng A có trống ngày đó vào lúc 16-18h
2. Check phòng đó có trống ngày 1/1 hay không
Phương án tối ưu là phương án mà việc kiểm tra đơn giản nhất và dữ liệu lưu ít nhất


Giải thích project:
Phương án:
 1. Lưu trữ ngày bắt đầu và ngày kết thúc dạng DateTime -> nhược khó check phòng trống theo thời gian
 2. Dùng kiểu lưu trữ 24 bit nhị phân 0,1 tương ứng với 24h ->các thao tác với bitwise có thể khó hiểu hơn
 3. Dùng bool[24] để thao tác với 24 h và chuyển array thành string để lưu trữ trong database -> thuận tiện, đơn giản, dễ thao tác -> chọn cách 3


Giải thích Code:
- bool[24] tương ứng 24h (true -> có thể booking, false -> không thể booking)
- Đặt phòng chia làm các trường hợp:
   + nếu khoảng cách lớn hớn hoặc bằng 1 ngày -> booking ngày
   + nếu khoảng cách nhỏ hơn 1 ngày và ngày bắt đàu và ngày kết thúc ở 2 ngày khác nhau -> chia làm 2 bước là booking ngày bắt đầu tới 23h và booking ngày kết thúc từ 0h tới giờ kết thúc.
   + nếu khoảng cách nhỏ hơn 1 ngày và ngày kết thúc trùng ngày bắt đầu -> booking theo giờ

 Giải thích chạy:
 - chương trình console app nên chỉ cần: F5 để mở teminal thao tác.
 - nhập RoomId: mã phòng
 - nhập 1 (chọn booking theo ngày), nhập 2 (chọn booking theo giờ)
 - nếu booking theo ngày thì chỉ cần nhập [năm-tháng-ngày] bắt đầu và kết thúc. Giá trị giờ-phút lưu trong data mặc định 00:00
 - nếu booking theo giờ cần nhập [năm-tháng-ngày giờ] bắt đầu và kết thúc. Giá trị phút k được xét trong ví dụ này nên mặc định là 00
 - lưu ý: 1 ngày có 24h tương ứng từ 0h tới 23h59. Vì vậy nếu muốn đặt thời gian từ 23h tới 24h trong 1 ngày thì phải nhập ngày kết thúc là [năm-tháng-(ngày+1) 00]
 - vì đây là code test các chức năng (k phải client layer) nên sau mỗi lần booking -> hiển thị danh sách các ngày tương ứng với roomId để check dễ dàng.
