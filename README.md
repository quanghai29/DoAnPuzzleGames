# DoAnPuzzleGames


ý tưởng đồ án:
- Sử dụng mẫu design method partern Links tham khảo : https://www.geeksforgeeks.org/template-method-design-pattern/ 
- ý tưởng chọn hình là nguyên dàng cầu thủ của đội tuyển Việt Nam
- Màn hình chính màu trắng để chơi game
- Màn hình màu đen để hiển thị hình ảnh gốc ban đầu , loạt select để người dùng chọn hình ảnh tùy ý , ( hoặc hiển thị thêm nút chọn ảnh để người dùng tự chọn)
- Màn hình màu hồng gồm 
	nút play , hiển thị đồng hồ đếm ,nút reset, save game
 	chọn chế độ chơi (dễ : 4x4, trung bình 6x6, khó 8x8) => sử dụng chuyền dữ liệu giữa hai màn hình
	tính điểm người dùng (nếu được)

- Hoạt động : Ban đầu người dùng vào chọn ảnh ở select để chơi , ảnh sau đó được cắt ra và xáo trộn (mặc định ở chế độ 4x4)
  sau đó người dùng nhấn nút play và bắt đầu chơi , đồng hồ chạy , người dùng có thể chọn chế độ chơi 
 đang chơi người dùng có thể reset lại nếu bí đường 
 đang chơi người dùng cũng có thể đổi chơi hình khác , hệ thống sẽ hỏi người dùng muốn lưu game hay không
 Chế độ tự động : sau khi người dùng chơi ở mức dễ pass hệ thống tự động tính điểm và nhảy đến chế độ trung bình , khó sau đố thông báo 
 người dùng đã hoàn thành với bức ảnh này ( có thể cho chạy qua bức khác nếu được) 

- Phân công công việc:
  Các mục chính:
  + Duy : Xáo hình , snap hình vào ô gần nhất (mục 1,3), báo thắng, tính điểm
  + Đồng : Select hình( hoặc tạo một của sổ mới có loạt hình để chọn) , đồng hồ bấm giờ, xử lý hiệu ứng thằng thua cuộc 
  + Hải : Store game, xử lý tinh chỉnh giao diện, tạo button để điều chỉnh đường đi
