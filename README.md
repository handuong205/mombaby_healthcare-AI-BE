<div align="center">
  <img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8"/>
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL"/>
  <img src="https://img.shields.io/badge/Entity_Framework-0078D4?style=for-the-badge&logo=.net&logoColor=white" alt="EF Core"/>
  <img src="https://img.shields.io/badge/Gemini_AI-8E75B2?style=for-the-badge&logo=googlebard&logoColor=white" alt="Gemini AI"/>
  
  <h1>MomOi - Hệ sinh thái chăm sóc sức khỏe Mẹ & Bé</h1>
  <p>Dự án EXE101: Nền tảng Y tế Ứng dụng Trí tuệ Nhân tạo (AI)</p>
</div>

## 📖 Bảng Mục lục
- [Giới thiệu dự án](#-giới-thiệu-dự-án)
- [Tính năng nổi bật (Theo Role)](#-tính-năng-nổi-bật-theo-role)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Hướng dẫn cài đặt (Getting Started)](#-hướng-dẫn-cài-đặt-getting-started)
- [Biến môi trường (Environment Variables)](#-biến-môi-trường-environment-variables)
- [Tài liệu API Chi tiết (API Documentation Sheet)](API_DOCUMENTATION.md)

---

## 🌟 Giới thiệu dự án
**MomOi** là một hệ sinh thái chăm sóc sức khỏe toàn diện dành cho phụ nữ mang thai, sau sinh và trẻ sơ sinh. Với sự hỗ trợ đắc lực từ **Trí tuệ nhân tạo (Google Gemini)**, hệ thống không chỉ cung cấp kiến thức chuẩn y khoa mà còn tự động phân tích tâm lý, cảnh báo nguy cơ trầm cảm (EPDS) và gợi ý thực đơn cá nhân hóa.

Hệ thống được thiết kế linh hoạt với kiến trúc **Role-Based Access Control (RBAC)** phục vụ 4 nhóm đối tượng chính: *Admin, Staff, Expert (Chuyên gia/Bác sĩ), và Mom (Mẹ)*.

---

## ✨ Tính năng nổi bật (Theo Role)

### 👩‍👧 Dành cho Mẹ (Mom)
- **Hồ sơ sức khỏe:** Lưu trữ chỉ số thai kỳ, cân nặng, nhật ký ăn dặm.
- **Thực đơn AI (Diet Plan):** Tự động sinh thực đơn dinh dưỡng 7 ngày dựa vào độ tuổi, cân nặng và **Khai báo dị ứng thực phẩm** của trẻ thông qua AI.
- **Trợ lý Tâm lý:** Đánh giá trầm cảm sau sinh (EPDS Score) và phân tích cảm xúc qua giọng nói (Voice Journal).
- **Tư vấn trực tiếp:** Khởi tạo chat 1-1 với Bác sĩ/Chuyên gia dinh dưỡng.
- **Premium (SuperMomVip):** Nâng cấp tài khoản qua Cổng thanh toán (MoMo/VNPAY).

### 👨‍⚕️ Dành cho Chuyên gia (Expert)
- **Duyệt công thức AI:** Kiểm tra và phê duyệt/từ chối các công thức ăn dặm do AI sinh ra để đảm bảo chuẩn y khoa.
- **Tư vấn trực tuyến:** Nhắn tin tư vấn sức khỏe trực tiếp với các Mẹ.

### 🛡️ Dành cho Quản trị viên (Admin & Staff)
- **Quản lý tài khoản:** Thêm/sửa/khóa tài khoản người dùng, phân quyền hệ thống.
- **Business Rule Engine:** Hệ thống quản lý các luật sức khỏe động (Ví dụ: Tự định nghĩa ngưỡng cảnh báo BMI, Huyết áp) mà không cần can thiệp vào code.
- **Tích hợp USDA:** Đồng bộ dữ liệu dinh dưỡng (Calories, Protein, Carbs, Fat) từ Bộ Nông nghiệp Hoa Kỳ.
- **Dashboard & Báo cáo:** Theo dõi biểu đồ sức khỏe và danh sách bệnh nhân có rủi ro cao.

---

## 💻 Công nghệ sử dụng
- **Backend Framework:** ASP.NET Core 8.0 (Web API)
- **Cơ sở dữ liệu:** PostgreSQL
- **ORM:** Entity Framework Core (Code-First Migration)
- **Xác thực (Authentication):** JWT (JSON Web Tokens) & ASP.NET Core Identity
- **Trí tuệ nhân tạo:** Google Gemini 1.5 Pro REST API
- **Real-time Communication:** SignalR (Chat)
- **Third-party API:** USDA FoodData Central

---

## 🚀 Hướng dẫn cài đặt (Getting Started)

### Yêu cầu hệ thống
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (Hoặc sử dụng Docker)
- Visual Studio 2022 / JetBrains Rider / VS Code

### Khởi chạy nhanh bằng Docker (Khuyến nghị - 1 lệnh duy nhất)
Chỉ cần cài đặt [Docker Desktop](https://www.docker.com/products/docker-desktop/) và chạy lệnh sau tại thư mục gốc:

```bash
docker compose up -d --build
```

Hệ thống sẽ tự động:
1. Đóng gói và chạy **PostgreSQL 15** (khởi tạo database `momoidb`).
2. Khởi tạo và chạy **MomOi.NutritionAPI** (Python FastAPI - port `8001`), tự động nạp seed data dinh dưỡng WHO/USDA.
3. Khởi tạo và chạy **MomOi.API** (.NET 8 - port `8000`), tự động apply EF Core Migration và nạp tài khoản admin mặc định.

### 🌐 Các địa chỉ truy cập sau khi chạy:
- **Swagger UI (.NET Core Backend)**: [http://localhost:8000/swagger](http://localhost:8000/swagger)
- **FastAPI Docs (Nutrition Engine)**: [http://localhost:8001/docs](http://localhost:8001/docs)
- **Kiểm tra trạng thái (Health Check)**:
  - Backend: [http://localhost:8000/health](http://localhost:8000/health)
  - Nutrition API: [http://localhost:8001/health](http://localhost:8001/health)

### 👤 Tài khoản Quản trị mặc định (Seed Data):
- **Email**: `admin@momoi.com`
- **Mật khẩu**: `Admin@123`

---

### Khởi chạy thủ công bằng CLI (.NET SDK cục bộ)
1. **Yêu cầu hệ thống:** .NET 8 SDK, PostgreSQL, Python 3.12 (nếu chạy Nutrition API).
2. **Cấu hình chuỗi kết nối:** Xem trong file `appsettings.json` hoặc tạo file `.env`.
3. **Chạy Migration:**
   ```bash
   dotnet ef database update --project MomOi.API
   ```
4. **Khởi động server:**
   ```bash
   dotnet run --project MomOi.API
   ```

---

## 🔑 Hướng dẫn cấu hình Biến môi trường (.env)

Hệ thống cung cấp sẵn file mẫu [`.env.example`](.env.example) và file cấu hình chạy ngay [`.env`](.env). Dưới đây là hướng dẫn chi tiết từng nhóm biến và thời điểm bạn nên sử dụng:

### 1. Cấu hình Cốt lõi (Bắt buộc)
| Biến | Giá trị mặc định | Giải thích & Khi nào cần dùng |
|:---|:---|:---|
| `POSTGRES_USER` | `postgres` | Tên tài khoản database PostgreSQL. |
| `POSTGRES_PASSWORD` | `postgres123` | Mật khẩu database PostgreSQL. |
| `POSTGRES_DB` | `momoidb` | Tên database chính cho cả 2 API. |
| `USDA_API_KEY` | `DEMO_KEY` | Key kết nối USDA FoodData Central. Mặc định `DEMO_KEY` đã đủ dùng để test (giới hạn 30 req/h). Nếu cần key riêng miễn phí (1.000 req/h), đăng ký tại [USDA FDC Signup](https://fdc.nal.usda.gov/api-key-signup.html). **Tuyệt đối không điền chuỗi bừa bãi** vì sẽ bị lỗi 403 Forbidden. |

### 2. Trí tuệ Nhân tạo (Google Gemini AI)
* **Biến:** `GEMINI_API_KEY`
* **Khi nào nên dùng?** Khi bạn muốn test các tính năng AI thực tế:
  - Phân tích bài test trầm cảm sau sinh (**EPDS Score**), sinh lời động viên thấu cảm.
  - Phân tích nhật ký cảm xúc giọng nói (**Voice Journal**).
  - Phân loại mức độ nguy hiểm của triệu chứng lâm sàng (**Symptom Triage**).
  - Tự động sinh thực đơn ăn dặm cá nhân hóa theo yêu cầu đặc biệt.
* **Nếu để trống?** Hệ thống **KHÔNG bị crash**. Code có sẵn cơ chế fallback trả về kết quả mẫu/mặc định.
* **Lấy ở đâu?** Miễn phí tại [Google AI Studio](https://aistudio.google.com/) -> Bấm **"Get API key"**.

### 3. Dịch vụ gửi Email (SMTP - MailKit)
* **Biến:** `Smtp__Host`, `Smtp__Port`, `Smtp__Username`, `Smtp__Password`, `Smtp__EnableSsl`
* **Khi nào nên dùng?** Khi bạn muốn kiểm thử luồng gửi email thật:
  - Gửi email xác thực khi đăng ký tài khoản mới.
  - Gửi mã OTP / link đặt lại mật khẩu (**Quên mật khẩu**).
  - Gửi email cảnh báo nguy cấp (**Critical Alerts**) đến người thân hoặc bác sĩ.
* **Nếu để trống?** Hệ thống ghi log cảnh báo và bỏ qua, API vẫn hoạt động bình thường.
* **Cách lấy:** Sử dụng tài khoản Gmail cá nhân: Bật *Xác thực 2 bước (2FA)* -> Vào mục [Mật khẩu ứng dụng (App Passwords)](https://myaccount.google.com/apppasswords) để tạo mật khẩu 16 ký tự điền vào `Smtp__Password`.

### 4. Dịch vụ tin nhắn SMS (Twilio)
* **Biến:** `Twilio__AccountSid`, `Twilio__AuthToken`, `Twilio__FromPhoneNumber`
* **Khi nào nên dùng?** Khi muốn test gửi tin nhắn SMS SOS/khẩn cấp về điện thoại thật khi phát hiện chỉ số sức khỏe nguy kịch.
* **Nếu để trống?** Hệ thống bỏ qua bước gửi SMS mà không báo lỗi.
* **Cách lấy:** Đăng ký tài khoản thử nghiệm tại [Twilio Console](https://www.twilio.com/) (được tặng sẵn ~15$ credit miễn phí).

### 5. Cổng thanh toán (MoMo & VNPay)
* **Biến:** `Payment__MoMo__*`, `Payment__VnPay__*`
* **Khi nào nên dùng?** Khi bạn phát triển đến tính năng nạp tiền / mua gói hội viên VIP (**SuperMomVip**) để sinh mã QR thanh toán và nhận webhook (IPN) kích hoạt VIP.
* **Nếu để trống?** Chỉ các API liên quan đến thanh toán (`/api/payment/*`) báo lỗi thiếu cấu hình, mọi tính năng y tế khác hoạt động bình thường.
* **Cách lấy:**
  - **VNPay:** Đăng ký tài khoản Sandbox tại [VNPay Sandbox](https://sandbox.vnpayment.vn/).
  - **MoMo:** Đăng ký tài khoản thử nghiệm tại [MoMo Developer](https://developers.momo.vn/).

### 6. Khóa mã hóa Token (JWT Authentication)
* **Biến:** `Jwt__PrivateKey`, `Jwt__PublicKey`
* **Khi nào nên dùng?**
  - **Môi trường Dev / Local:** **Cứ để trống!** Backend đã tự động sinh một cặp khóa RSA tạm trong bộ nhớ RAM (`GetDeveloperKey()`) để bạn đăng nhập và phân quyền ngay lập tức.
  - **Môi trường Production:** Điền cặp khóa RSA 2048-bit (chuẩn PEM) tạo qua OpenSSL để phiên đăng nhập của người dùng không bị mất khi restart server hoặc khi chạy nhiều container song song.
  - **CẢNH BÁO:** **Không được điền chữ ngẫu nhiên** (như `123456`) vì sẽ gây lỗi định dạng giải mã PEM.

---

<div align="center">
  <i>Được phát triển với ❤️ cho dự án Capstone Y tế thông minh.</i>
</div>
#   m o m b a b y _ h e a l t h c a r e - A I - B E  
 #   m o m b a b y _ h e a l t h c a r e - A I - B E  
 