# Tiến độ dự án ChatHoTro (Đề tài 16)

## Mục tiêu tổng quát
Hệ thống chat hỗ trợ trực tuyến phân tán bằng C#, ASP.NET Core Minimal API, SignalR, SQL Server, RabbitMQ và Worker Service, chạy bằng Docker Compose; tập trung hỗ trợ khách hàng đặt hàng và xử lý đơn hàng trên sàn thương mại điện tử (tra cứu trạng thái đơn, đổi địa chỉ nhận hàng, lỗi thanh toán, mã giảm giá, v.v.). Hệ thống giữ hai vai trò `Customer`/`Agent` và vòng đời hội thoại `Waiting` -> `Active` -> `Closed`.

## Danh sách đầu việc (Checklist)
- [x] Giai đoạn 1: `Contracts` (`Models.cs`: ConversationStatus, ParticipantRole, DTOs Conversation/Participant/Message, MessageSentEvent, ConversationClosedEvent)
- [x] Giai đoạn 2: `ChatApi` - EF Core SQL Server + 5 Minimal API (`POST /conversations`, `GET /conversations`, `GET /conversations/{id}`, `GET /conversations/{id}/messages`, `POST /conversations/{id}/close`)
- [ ] Giai đoạn 3: `ChatApi` - SignalR Hub (`/hubs/chat`) phân nhóm theo phiên, join/send/close realtime + publish RabbitMQ
- [ ] Giai đoạn 4: `HistoryWorker` - Consume RabbitMQ, lưu/cập nhật lịch sử hội thoại, idempotent & ack/nack phục hồi khi dừng/bật lại
- [ ] Giai đoạn 5: `client-web` (Customer & Agent) + `Dockerfile` từng service + `docker-compose.yml` + `.env.example`
- [ ] Giai đoạn 6: `README.md`, khung báo cáo BTL và kiểm thử build/chạy tổng thể

## Nhật ký hiện tại
- Giai đoạn đang làm: Giai đoạn 3 - `ChatApi` SignalR Hub và RabbitMQ Publisher
- Việc vừa hoàn thành: Hoàn thành Giai đoạn 2: EF Core SQL Server, migration khởi tạo, 5 Minimal API, CORS/OpenAPI; build toàn solution thành công
- Vấn đề / lưu ý đang gặp: Chưa chạy tích hợp với SQL Server đang hoạt động. `dotnet-ef` bị Windows Application Control chặn khi nạp assembly trong quá trình tạo migration; migration ban đầu được tạo trong source và đã qua build.