# Tiến độ dự án ChatHoTro (Đề tài 16)

## Mục tiêu tổng quát
Hệ thống chat hỗ trợ trực tuyến phân tán bằng C#, ASP.NET Core Minimal API, SignalR, SQL Server, RabbitMQ và Worker Service, chạy bằng Docker Compose.

## Danh sách đầu việc (Checklist)
- [x] Giai đoạn 1: `Contracts` (`Models.cs`: ConversationStatus, ParticipantRole, DTOs Conversation/Participant/Message, MessageSentEvent, ConversationClosedEvent)
- [ ] Giai đoạn 2: `ChatApi` - EF Core SQL Server + 5 Minimal API (`POST /conversations`, `GET /conversations`, `GET /conversations/{id}`, `GET /conversations/{id}/messages`, `POST /conversations/{id}/close`)
- [ ] Giai đoạn 3: `ChatApi` - SignalR Hub (`/hubs/chat`) phân nhóm theo phiên, join/send/close realtime + publish RabbitMQ
- [ ] Giai đoạn 4: `HistoryWorker` - Consume RabbitMQ, lưu/cập nhật lịch sử hội thoại, idempotent & ack/nack phục hồi khi dừng/bật lại
- [ ] Giai đoạn 5: `client-web` (Customer & Agent) + `Dockerfile` từng service + `docker-compose.yml` + `.env.example`
- [ ] Giai đoạn 6: `README.md`, khung báo cáo BTL và kiểm thử build/chạy tổng thể

## Nhật ký hiện tại
- Giai đoạn đang làm: Giai đoạn 2 - `ChatApi` (EF Core SQL Server + 5 Minimal API)
- Việc vừa hoàn thành: Hoàn thiện Giai đoạn 1 theo đặc tả chi tiết: hợp nhất enums, DTOs và RabbitMQ events trong `src/Contracts/Models.cs`; build thành công
- Vấn đề / lưu ý đang gặp: Chưa ghi nhận