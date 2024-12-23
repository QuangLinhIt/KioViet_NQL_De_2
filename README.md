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
bool[24] tương ứng 24h nếu true -> có thể booking, false -> không thể booking

