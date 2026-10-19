# 📚 Bảng Hướng Dẫn Sử Dụng Toàn Bộ API (API Sheet Reference)
> **Hệ sinh thái Chăm sóc Sức khỏe Mẹ & Bé MomOi**  
> Bao gồm: **MomOi.API (.NET 8 Web API)** và **MomOi.NutritionAPI (Python FastAPI)**

---

## 📌 1. THÔNG TIN CHUNG & CẤU HÌNH TRUY CẬP

| Dịch vụ | Base URL Cục bộ / Docker | Tài liệu Swagger / Docs | Cổng nội bộ |
| :--- | :--- | :--- | :--- |
| **MomOi.API (.NET 8)** | `http://localhost:8000` | `http://localhost:8000/swagger` | Port 80 (Docker) / 8000 (Host) |
| **MomOi.NutritionAPI (FastAPI)** | `http://localhost:8001` | `http://localhost:8001/docs` | Port 8001 (Host & Docker) |
| **PostgreSQL Database** | `localhost:5434` (User: `postgres`, Pass: `postgres123`, DB: `momoidb`) | N/A | Port 5432 (Internal) |

### Chuẩn dữ liệu phản hồi (.NET Core API Response Format)
Tất cả các API của .NET Core đều tuân thủ cấu trúc chuẩn bọc qua `ApiResponse<T>`:
```json
{
  "success": true,
  "data": { ... },
  "message": "Thông báo kết quả thành công",
  "errors": null,
  "errorCode": null
}
```

### Cơ chế Xác thực (JWT Authentication)
- Sau khi gọi API `/api/auth/login` hoặc `/api/auth/register`, lấy `data.accessToken`.
- Truyền vào Header của mọi request yêu cầu đăng nhập:
  ```http
  Authorization: Bearer <your_access_token>
  ```
- **Tài khoản Quản trị mặc định (Seed data):**
  - Email: `admin@momoi.com` | Password: `Admin@123` (Role: `Admin`)
  - Email: `staff@momoi.com` | Password: `Staff@123` (Role: `Staff`)
  - Email: `expert@momoi.com` | Password: `Expert@123` (Role: `Expert`)
  - Email: `mom@momoi.com` | Password: `Mom@123` (Role: `Mom`)

---

## 🔷 PHẦN A: DANH SÁCH API CỦA MOMOI.API (.NET 8)

---

### 1. Phân hệ Xác thực & Tài khoản (`/api/auth`)

#### 1.1 Đăng ký tài khoản (`POST /api/auth/register`)
- **Mô tả:** Đăng ký người dùng mới (mặc định Role `Mom`), tự khởi tạo hồ sơ sức khỏe rỗng.
- **Quyền:** Public (Không cần Token).
- **Body (JSON):**
  ```json
  {
    "fullName": "Nguyễn Thị Mai",
    "email": "mai.nguyen@example.com",
    "password": "Password123@",
    "phoneNumber": "0987654321"
  }
  ```
- **Cách dùng (cURL):**
  ```bash
  curl -X POST http://localhost:8000/api/auth/register \
    -H "Content-Type: application/json" \
    -d '{"fullName":"Nguyễn Thị Mai","email":"mai.nguyen@example.com","password":"Password123@","phoneNumber":"0987654321"}'
  ```

#### 1.2 Đăng nhập (`POST /api/auth/login`)
- **Mô tả:** Kiểm tra thông tin đăng nhập, trả về Access Token (15-60p) và Refresh Token (7 ngày).
- **Quyền:** Public.
- **Body (JSON):**
  ```json
  {
    "email": "mai.nguyen@example.com",
    "password": "Password123@"
  }
  ```

#### 1.3 Làm mới Token (`POST /api/auth/refresh`)
- **Mô tả:** Cấp lại Access Token mới khi token cũ hết hạn bằng Refresh Token.
- **Quyền:** Public.
- **Body (JSON):**
  ```json
  {
    "refreshToken": "string_refresh_token"
  }
  ```

#### 1.4 Đăng xuất (`POST /api/auth/logout`)
- **Mô tả:** Hủy phiên làm việc và thu hồi Refresh Token trong DB.
- **Quyền:** Yêu cầu Token (`Bearer Token`).

---

### 2. Hồ sơ Người dùng & Chỉ số Sức khỏe (`/api/user-profile`)

#### 2.1 Xem hồ sơ sức khỏe (`GET /api/user-profile`)
- **Mô tả:** Lấy thông tin chi tiết: tuần thai, cân nặng, chiều cao, BMI, huyết áp cơ sở, dị ứng, bệnh lý nền.
- **Quyền:** Yêu cầu Token (`Bearer Token`).

#### 2.2 Cập nhật hồ sơ sức khỏe (`PUT /api/user-profile`)
- **Mô tả:** Cập nhật các chỉ số cơ thể, giai đoạn mang thai/sau sinh.
- **Quyền:** Yêu cầu Token.
- **Body (JSON):**
  ```json
  {
    "currentWeight": 58.5,
    "height": 162.0,
    "bloodPressureSystolic": 115,
    "bloodPressureDiastolic": 75,
    "journeyStage": "Pregnancy",
    "medicalConditions": "Tiểu đường thai kỳ nhẹ"
  }
  ```

---

### 3. Phân hệ Theo dõi Thai kỳ (`/api/pregnancy`)

#### 3.1 Khởi tạo lộ trình thai kỳ (`POST /api/pregnancy/setup`)
- **Mô tả:** Nhập ngày đầu kỳ kinh cuối (LMP) hoặc ngày dự sinh (Due Date), hệ thống tự tính tuần thai, tam cá nguyệt (Trimester) và các mốc quan trọng.
- **Quyền:** Yêu cầu Token.
- **Body (JSON):**
  ```json
  {
    "lastMenstrualPeriod": "2026-01-15T00:00:00Z",
    "dueDate": "2026-10-22T00:00:00Z"
  }
  ```

#### 3.2 Lấy thông tin thai kỳ tuần hiện tại (`GET /api/pregnancy/this-week`)
- **Mô tả:** Trả về kích thước ước tính của bé (ví dụ: quả bơ, quả bưởi), mức độ phát triển tế bào/cơ quan, lời khuyên dinh dưỡng cho tuần đó.
- **Quyền:** Yêu cầu Token.

#### 3.3 Ghi nhật ký thức ăn mẹ bầu (`POST /api/pregnancy/food-log`)
- **Mô tả:** Kiểm tra độ an toàn của thực phẩm đối với mẹ mang thai (cảnh báo thực phẩm tái sống, chất kích thích...).
- **Body (JSON):**
  ```json
  {
    "foods": ["Cá hồi áp chảo", "Rau bina luộc", "Sữa chua tiệt trùng"]
  }
  ```

#### 3.4 Gợi ý thực đơn 7 ngày cho mẹ (`GET /api/pregnancy/meal-plan`)
- **Mô tả:** Trả về thực đơn tuần cá nhân hóa theo tuần thai (Query: `?week=14`).
- **Quyền:** Yêu cầu Token (Gói Modern Mom hoặc VIP).

#### 3.5 Ghi nhận cân nặng thai kỳ (`POST /api/pregnancy/weight-log`)
- **Mô tả:** Ghi nhận cân nặng mẹ và kiểm tra tốc độ tăng cân bất thường (Cảnh báo tiền sản giật / suy dinh dưỡng bào thai).
- **Body (JSON):**
  ```json
  {
    "weightKg": 60.2,
    "date": "2026-09-19T00:00:00Z"
  }
  ```

#### 3.6 Xem lịch sử cân nặng mẹ (`GET /api/pregnancy/weight-logs`)
- **Mô tả:** Lấy danh sách toàn bộ các mốc cân nặng để vẽ biểu đồ tăng cân thai kỳ.

#### 3.7 Ghi nhận bài tập vận động / đếm bước (`POST /api/pregnancy/exercise-log`)
- **Mô tả:** Ghi nhận số bước chân, bài tập yoga/bơi lội và cảnh báo thiếu vận động theo quy tắc BR03.
- **Body (JSON):**
  ```json
  {
    "stepCount": 4500,
    "exerciseType": "Đi bộ nhẹ nhàng",
    "durationMinutes": 30
  }
  ```

#### 3.8 Lấy kế hoạch bài tập theo tam cá nguyệt (`GET /api/pregnancy/exercise-plan`)
- **Mô tả:** Hướng dẫn các bài tập thể dục an toàn theo từng tuần thai.

#### 3.9 Lấy tổng số bước chân hôm nay (`GET /api/pregnancy/today-steps`)
- **Mô tả:** Xem nhanh số bước chân đã hoàn thành trong ngày.

---

### 4. Phân hệ Phục hồi Sau sinh & Tâm lý EPDS (`/api/postpartum`)

#### 4.1 Thiết lập hồ sơ sau sinh (`POST /api/postpartum/setup`)
- **Mô tả:** Nhập ngày sinh con, phương pháp sinh (thường/mổ), tình trạng cho con bú để xây dựng lộ trình hồi phục tử cung/vết khâu.
- **Body (JSON):**
  ```json
  {
    "deliveryDate": "2026-08-01T00:00:00Z",
    "deliveryType": "cesarean",
    "isBreastfeeding": true
  }
  ```

#### 4.2 Làm bài kiểm tra trầm cảm sau sinh EPDS (`POST /api/postpartum/epds`)
- **Mô tả:** Gửi 10 câu trả lời (điểm 0-3 mỗi câu). Hệ thống tính điểm tổng (0-30), chạy luật **BR05** và gọi **Gemini AI** sinh lời phản hồi động viên ấm áp.
- **Body (JSON):**
  ```json
  {
    "answers": [1, 0, 2, 1, 0, 1, 2, 0, 1, 1]
  }
  ```

#### 4.3 Phân tích nhật ký cảm xúc qua giọng nói AI (`POST /api/postpartum/epds/voice-journal`)
- **Mô tả:** Upload đoạn ghi âm tâm sự của người mẹ dạng Base64. Gemini AI phân tích mức độ lo âu, cảm xúc ẩn và trả về điểm rủi ro.
- **Body (JSON):**
  ```json
  {
    "audioBase64": "<base64_audio_string>",
    "mimeType": "audio/mp3"
  }
  ```

#### 4.4 Ghi nhật ký cho con bú (`POST /api/postpartum/breastfeeding-log`)
- **Mô tả:** Ghi nhận thời gian và bên ngực bé bú (left / right / both).
- **Body (JSON):**
  ```json
  {
    "side": "both",
    "durationMinutes": 25,
    "time": "2026-09-19T10:00:00Z"
  }
  ```

#### 4.5 Kế hoạch bài tập phục hồi sàn chậu (`GET /api/postpartum/recovery-plan?day=30`)
- **Mô tả:** Bài tập Kegel, phục hồi cơ bụng và sàn chậu theo số ngày sau sinh.

#### 4.6 Xem kết quả EPDS gần nhất (`GET /api/postpartum/epds/latest`)
- **Mô tả:** Lấy điểm số bài test trầm cảm gần nhất để theo dõi chuyển biến tâm lý.

---

### 5. Phân hệ Hồ sơ Bé & Dinh dưỡng Ăn dặm (`/api/baby`)

#### 5.1 Tạo hồ sơ cho bé (`POST /api/baby/profile`)
- **Mô tả:** Nhập tên bé, ngày sinh, giới tính, cân nặng, chiều cao lúc sinh và danh sách dị ứng.
- **Body (JSON):**
  ```json
  {
    "babyName": "Bé Bắp",
    "dateOfBirth": "2026-03-10T00:00:00Z",
    "gender": "Female",
    "currentWeightKg": 7.8,
    "currentHeightCm": 68.5,
    "allergies": ["Hải sản", "Đậu phộng"]
  }
  ```

#### 5.2 Danh sách hồ sơ các bé của mẹ (`GET /api/baby/profiles`)
- **Mô tả:** Lấy toàn bộ các bé thuộc quyền quản lý của tài khoản hiện tại.

#### 5.3 Cập nhật hồ sơ bé (`PUT /api/baby/{id}`)
- **Mô tả:** Chỉnh sửa thông tin bé (cân nặng, dị ứng mới phát hiện...).

#### 5.4 Gợi ý thực đơn ăn dặm 1 ngày (`GET /api/baby/{id}/menu/daily?force=false`)
- **Mô tả:** Gọi sang Python Nutrition Engine: tự động tính năng lượng, lọc trừ đồ dị ứng của bé và đưa ra 3 bữa ăn dặm + 1 bữa phụ chuẩn WHO.

#### 5.5 Gợi ý thực đơn ăn dặm 7 ngày (`GET /api/baby/{id}/menu/weekly?force=false`)
- **Mô tả:** Sinh thực đơn ăn dặm đa dạng cho cả tuần, không trùng lặp món.

#### 5.6 Ghi nhận mốc tăng trưởng của bé (`POST /api/baby/{id}/growth`)
- **Mô tả:** Ghi nhận cân nặng/chiều cao mới. Hệ thống so sánh với chuẩn **WHO Growth Standards (Z-score)** để đánh giá thừa/thiếu cân.
- **Body (JSON):**
  ```json
  {
    "date": "2026-09-19T00:00:00Z",
    "weightKg": 8.1,
    "heightCm": 69.2,
    "notes": "Bé ăn ngoan, ngủ tốt"
  }
  ```

#### 5.7 Xóa mốc tăng trưởng (`DELETE /api/baby/{id}/growth/{recordId}`)
- **Mô tả:** Xóa bản ghi tăng trưởng bị nhập nhầm.

---

### 6. Phân hệ Chu kỳ Kinh nguyệt & Hỗ trợ Sinh sản (`/api/fertility`)

#### 6.1 Ghi nhật ký chu kỳ kinh nguyệt (`POST /api/fertility/cycle-log`)
- **Mô tả:** Nhập ngày bắt đầu kỳ kinh, độ dài chu kỳ (Cycle Length) và triệu chứng. Tự động tính ngày rụng trứng tiếp theo.
- **Body (JSON):**
  ```json
  {
    "periodStartDate": "2026-09-01T00:00:00Z",
    "cycleLength": 28,
    "symptoms": ["Đau lưng nhẹ", "Mệt mỏi"]
  }
  ```

#### 6.2 Xem lịch rụng trứng & ngày thụ thai (`GET /api/fertility/calendar?month=2026-09`)
- **Mô tả:** Trả về ma trận các ngày an toàn, ngày rụng trứng và "cửa sổ thụ thai" (Fertile Window) trong tháng.

#### 6.3 Kiểm tra tình trạng rụng trứng hôm nay (`GET /api/fertility/ovulation-today`)
- **Mô tả:** Kiểm tra xem hôm nay có phải là ngày dễ thụ thai nhất không (kích hoạt quy tắc BR01).

#### 6.4 Lập lộ trình hỗ trợ sinh sản IVF (`POST /api/fertility/ivf-timeline`)
- **Mô tả:** Tạo lịch tiêm thuốc kích trứng, chọc hút trứng, chuyển phôi theo phác đồ (long/short/antagonist). Yêu cầu gói Modern Mom.
- **Body (JSON):**
  ```json
  {
    "ivfStartDate": "2026-10-01T00:00:00Z",
    "protocol": "antagonist"
  }
  ```

---

### 7. Phân hệ Theo dõi Chỉ số Sinh tồn Hàng ngày (`/api/daily-monitoring`)

#### 7.1 Ghi nhận / Cập nhật chỉ số hôm nay (`POST /api/daily-monitoring`)
- **Mô tả:** Ghi nhận huyết áp, đường huyết, nhiệt độ, lượng nước uống, giờ ngủ và đánh giá luật an toàn y tế.
- **Body (JSON):**
  ```json
  {
    "bloodPressureSystolic": 120,
    "bloodPressureDiastolic": 80,
    "bloodGlucose": 95.0,
    "bodyTemperature": 36.8,
    "waterIntakeMl": 2200,
    "sleepHours": 7.5,
    "mood": "Vui vẻ, thoải mái"
  }
  ```

#### 7.2 Lấy dữ liệu theo dõi hôm nay (`GET /api/daily-monitoring/today`)
- **Mô tả:** Xem lại các chỉ số đã nhập trong ngày hiện tại.

#### 7.3 Lịch sử chỉ số sinh tồn (`GET /api/daily-monitoring/history?limit=30`)
- **Mô tả:** Lấy danh sách 30 ngày gần nhất để hiển thị đồ thị nhịp tim/huyết áp.

#### 7.4 Phân tích tổng hợp chỉ số (`GET /api/daily-monitoring/insights?days=7`)
- **Mô tả:** Tính mức huyết áp trung bình, độ ổn định đường huyết trong 7 ngày qua.

---

### 8. Phân hệ Lối sống & Sức khỏe Tinh thần (`/api/lifestyle`)

#### 8.1 Ghi nhận lối sống hôm nay (`POST /api/lifestyle/entry`)
- **Mô tả:** Nhập mức độ stress, chất lượng giấc ngủ, bài tập thiền. Hệ thống chấm điểm **Health Score (0-100)**.
- **Body (JSON):**
  ```json
  {
    "stressLevel": 3,
    "sleepQuality": 4,
    "mindfulnessMinutes": 15,
    "waterIntakeLiters": 2.5
  }
  ```

#### 8.2 Xem lối sống hôm nay (`GET /api/lifestyle/today`)
#### 8.3 Lịch sử lối sống (`GET /api/lifestyle/history?days=30`)
#### 8.4 Cảnh báo lối sống nghiêm trọng (`GET /api/lifestyle/alerts`)
- **Mô tả:** Liệt kê các cảnh báo High-severity về mất ngủ kéo dài hoặc stress cao trong 7 ngày qua.
#### 8.5 Báo cáo tổng kết lối sống (`GET /api/lifestyle/summary`)
- **Mô tả:** Trả về điểm trung bình, chuỗi ngày duy trì thói quen tốt (streak count) và dữ liệu biểu đồ radar.

---

### 9. Phân hệ Ghi nhận Triệu chứng & AI Triage (`/api/symptoms`)

#### 9.1 Nhập triệu chứng & Phân loại cấp cứu AI (`POST /api/symptoms`)
- **Mô tả:** Mẹ nhập triệu chứng bất thường (ví dụ: "chóng mặt, ra máu nhẹ, đau bụng dưới"). Gemini AI phân tích mức độ nghiêm trọng (0-100). Nếu $\ge 70$, hệ thống tự động kích hoạt **Critical Alert** gửi đến bác sĩ.
- **Body (JSON):**
  ```json
  {
    "symptomName": "Đau đầu dữ dội và hoa mắt",
    "description": "Bị đau đầu kéo dài 3 tiếng kèm mắt nhìn mờ mờ",
    "severityUserRating": 4
  }
  ```

#### 9.2 Danh sách triệu chứng đã ghi nhận (`GET /api/symptoms?minSeverity=50`)
- **Mô tả:** Lọc triệu chứng theo mức độ nghiêm trọng.

#### 9.3 Chi tiết một triệu chứng (`GET /api/symptoms/{id}`)
- **Mô tả:** Xem lại phân tích và khuyến nghị của AI cho triệu chứng đó.

---

### 10. Phân hệ Nhắc nhở Uống thuốc (`/api/medications`)

#### 10.1 Tạo lịch nhắc uống thuốc (`POST /api/medications`)
- **Mô tả:** Tạo lịch uống vitamin, sắt, canxi, thuốc nội tiết. Background Worker sẽ tự động gửi thông báo đúng giờ.
- **Body (JSON):**
  ```json
  {
    "medicationName": "Viên sắt Blackmores",
    "dosage": "1 viên",
    "scheduledTimes": ["08:00", "13:00"],
    "startDate": "2026-09-19T00:00:00Z",
    "endDate": "2026-11-19T00:00:00Z",
    "instructions": "Uống sau ăn 30 phút, không uống cùng sữa"
  }
  ```

#### 10.2 Xem danh sách thuốc đang dùng (`GET /api/medications`)
#### 10.3 Xác nhận đã uống / bỏ lỡ thuốc (`POST /api/medications/{id}/adherence`)
- **Body (JSON):**
  ```json
  {
    "date": "2026-09-19T00:00:00Z",
    "scheduledTime": "08:00",
    "status": "Taken"
  }
  ```
#### 10.4 Xóa lịch uống thuốc (`DELETE /api/medications/{id}`)

---

### 11. Phân hệ Dinh dưỡng Mẹ & Thực đơn AI (`/api/diet` & `/api/mom`)

#### 11.1 Sinh thực đơn tuần chuẩn (`POST /api/diet/generate`)
- **Mô tả:** Tự sinh kế hoạch ăn uống 7 ngày cho mẹ.

#### 11.2 Xem thực đơn hiện tại (`GET /api/diet`)
#### 11.3 Cập nhật thực đơn (`PUT /api/diet`)
#### 11.4 Nhờ Gemini AI sáng tạo món ăn mới (`POST /api/diet/ai-generate`)
- **Mô tả:** Mẹ nhập nguyên liệu đang có sẵn trong tủ lạnh (ví dụ: "ức gà, bí đỏ, đậu phụ"), AI sinh ngay món ăn bổ dưỡng phù hợp.
- **Body (JSON):**
  ```json
  {
    "availableIngredients": ["Ức gà", "Bí ngô", "Nấm"],
    "preferences": "Ít dầu mỡ, nhiều đạm, vị thanh mát"
  }
  ```

#### 11.5 Quản lý danh sách thực phẩm dị ứng của Mẹ (`GET` / `POST` / `DELETE` tại `/api/mom/allergies`)
- **POST /api/mom/allergies**: Khai báo dị ứng (`{"allergenName": "Tôm cua", "severity": "High"}`).
- **GET /api/mom/allergies**: Xem các dị ứng đã khai báo.
- **DELETE /api/mom/allergies/{id}**: Xóa dị ứng.

---

### 12. Phân hệ Kho Công thức Món ăn (`/api/recipes`)

#### 12.1 Danh sách công thức cá nhân / đã lưu (`GET /api/recipes/my?isSaved=true&page=1&limit=20`)
#### 12.2 Chi tiết một công thức (`GET /api/recipes/{recipeId}`)
#### 12.3 Đánh dấu Yêu thích / Bỏ lưu công thức (`PATCH /api/recipes/{recipeId}/save`)
#### 12.4 Lấy hồ sơ dinh dưỡng phân loại mẹ bầu (`GET /api/recipes/profiles/current`)

---

### 13. Phân hệ Dành cho Bác sĩ / Chuyên gia Y tế (`/api/expert`)
> **Yêu cầu Role:** `Expert`, `Staff`, hoặc `Admin`

#### 13.1 Lấy danh sách công thức AI chờ duyệt (`GET /api/expert/recipes/pending`)
- **Mô tả:** Các công thức do AI sinh ra cần Bác sĩ dinh dưỡng kiểm duyệt trước khi đưa vào kho chung.

#### 13.2 Xem tất cả công thức phân nhóm Mẹ / Bé (`GET /api/expert/recipes/all`)
#### 13.3 Phê duyệt / Từ chối công thức ăn dặm (`PATCH /api/expert/recipes/{recipeId}/review`)
- **Body (JSON):**
  ```json
  {
    "isApproved": true,
    "expertNote": "Công thức giàu sắt, độ nhuyễn rất phù hợp cho bé từ 7 tháng tuổi."
  }
  ```

#### 13.4 Lấy danh sách Mẹ được phân công tư vấn (`GET /api/expert/moms`)
#### 13.5 Gửi lời dặn dò / tin nhắn tư vấn cho Mẹ (`POST /api/expert/moms/{momId}/consult`)
- **Body (JSON):**
  ```json
  {
    "message": "Chỉ số đường huyết của bạn hơi cao, hãy hạn chế ăn đồ ngọt và đi bộ 20 phút sau ăn nhé.",
    "recommendationCategory": "Dinh dưỡng"
  }
  ```

---

### 14. Phân hệ Quản trị Hệ thống & Luật Y tế Động (`/api/admin`)
> **Yêu cầu Role:** `Admin` (hoặc `Staff` với một số endpoint xem báo cáo)

#### 14.1 Quản lý Người dùng
- **GET /api/admin/users**: Lấy danh sách tất cả tài khoản, vai trò và trạng thái khóa.
- **POST /api/admin/users/create**: Tạo tài khoản cho nhân viên y tế (Staff / Expert).
  ```json
  {
    "fullName": "Bác sĩ Lê Hoàng",
    "email": "dr.hoang@momoi.com",
    "password": "DoctorPass123@",
    "role": "Expert"
  }
  ```
- **PATCH /api/admin/users/{userId}/lock**: Khóa tài khoản vi phạm.
- **PATCH /api/admin/users/{userId}/unlock**: Mở khóa tài khoản.

#### 14.2 Động cơ Quy tắc Y tế (Dynamic Business Rules Engine)
- **GET /api/admin/rules**: Xem tất cả các quy tắc cảnh báo tự động.
- **POST /api/admin/rules**: Định nghĩa quy tắc y tế mới.
  ```json
  {
    "ruleCode": "BR_BP_HIGH",
    "ruleName": "Huyết áp tâm thu cao",
    "category": "VitalSigns",
    "conditionExpression": "SystolicBP >= 140",
    "severity": "High",
    "alertMessage": "Huyết áp của bạn đang ở ngưỡng cao nguy hiểm (>= 140 mmHg). Vui lòng nghỉ ngơi và liên hệ cơ sở y tế!"
  }
  ```
- **PUT /api/admin/rules/{id}**: Sửa ngưỡng/nội dung quy tắc.
- **DELETE /api/admin/rules/{id}**: Xóa quy tắc.

#### 14.3 Đồng bộ dữ liệu dinh dưỡng USDA
- **POST /api/admin/usda/sync**:
  ```json
  {
    "foodKeywords": ["Salmon", "Sweet potato", "Spinach", "Avocado"]
  }
  ```

#### 14.4 Báo cáo & Danh sách nguy cơ cao
- **GET /api/admin/users/risk**: Lấy danh sách các mẹ bầu đang kích hoạt cảnh báo nguy cơ cao (High Severity).
- **GET /api/admin/reports/summary**: Báo cáo tổng quan về xu hướng stress, tỷ lệ trầm cảm và các quy tắc bị vi phạm nhiều nhất.

---

### 15. Phân hệ Cảnh báo & Real-time SignalR (`/api/alerts` & `/hubs/alerts`)

#### 15.1 Lấy danh sách thông báo của tôi (`GET /api/alerts?status=Unread`)
#### 15.2 Đổi trạng thái thông báo (`PATCH /api/alerts/{id}/status`)
- **Body:** `{"status": "Resolved"}` hoặc `{"status": "Read"}`
#### 15.3 Xóa thông báo (`DELETE /api/alerts/{id}`)
#### 15.4 Đánh dấu giải quyết rủi ro (`POST /api/alerts/{id}/resolve`)
#### 15.5 Kết nối Realtime SignalR WebSocket:
- **Endpoint Hub:** `http://localhost:8000/hubs/alerts`
- **Xác thực:** Truyền qua Query parameter `?access_token=<jwt_token>`.
- **Sự kiện nhận:** Lắng nghe event `ReceiveAlert` để hiển thị popup cảnh báo khẩn cấp ngay khi đo chỉ số vượt ngưỡng.

---

### 16. Phân hệ Cổng thanh toán MoMo & VNPay (`/api/payment`)

#### 16.1 Lấy bảng giá các gói hội viên (`GET /api/payment/plans`)
- **Quyền:** Public.
- **Kết quả:** Trả về danh sách gói VIP (`SuperMomVip` theo tháng, năm, v.v.).

#### 16.2 Khởi tạo thanh toán (`POST /api/payment/create`)
- **Mô tả:** Sinh link thanh toán chuyển hướng sang MoMo hoặc VNPay.
- **Body (JSON):**
  ```json
  {
    "planId": "SuperMomVip_Monthly",
    "gateway": "VnPay" // Hoặc "MoMo"
  }
  ```
- **Kết quả trả về:** Chứa `paymentUrl`. Frontend chuyển hướng trình duyệt của người dùng đến URL này.

#### 16.3 Webhook IPN (Server-to-Server Callback)
- **VNPay IPN**: `GET /api/payment/vnpay-ipn`
- **MoMo IPN**: `POST /api/payment/momo-ipn`
- *(Tự động xác thực chữ ký bảo mật và nâng cấp gói VIP khi khách trả tiền thành công).*

#### 16.4 Kiểm tra trạng thái đơn hàng (`GET /api/payment/status/{orderCode}`)
- **Mô tả:** Frontend thăm dò (poll) kết quả giao dịch thanh toán.

---

### 17. Dashboard & Health Check

#### 17.1 Tổng quan Dashboard Mẹ (`GET /api/dashboard`)
- **Mô tả:** 1 API duy nhất trả về toàn bộ dữ liệu trang chủ: cân nặng, tuần thai, nhắc nhở uống thuốc hôm nay, cảnh báo gần nhất và thực đơn trong ngày.

#### 17.2 Health Check (`GET /health`)
- **Mô tả:** Kiểm tra tình trạng kết nối tới cơ sở dữ liệu PostgreSQL. Trả về `Healthy` hoặc `Unhealthy`.

---

## 🐍 PHẦN B: DANH SÁCH API CỦA MOMOI.NUTRITIONAPI (FASTAPI)

> **Base URL:** `http://localhost:8001`  
> **Swagger Interactive Docs:** `http://localhost:8001/docs`

---

### 1. Dinh dưỡng USDA & Tra cứu Dữ liệu

#### 1.1 Ingest hàng loạt nguyên liệu từ USDA (`POST /api/nutrition/ingest`)
- **Mô tả:** Kéo dữ liệu dinh dưỡng nguyên liệu tiếng Anh từ USDA FDC, tự động dịch tên tiếng Việt và phân loại nhóm thực phẩm.
- **Body (JSON):**
  ```json
  {
    "food_names_en": ["Beef", "Pumpkin", "Salmon", "Egg yolk"]
  }
  ```

#### 1.2 Tra cứu dinh dưỡng nguyên liệu trực tiếp (`GET /api/nutrition/ingredient/{name_en}`)
- **Mô tả:** Ví dụ: `GET /api/nutrition/ingredient/Salmon` trả về chi tiết Calories, Protein, Sắt, Kẽm, Omega-3.

---

### 2. Khuyến nghị Thực đơn Ăn dặm Chuẩn WHO (Recommendation Engine)

#### 2.1 Sinh thực đơn 1 ngày theo tham số truyền vào (`POST /api/menu/daily`)
- **Mô tả:** Độc lập với DB người dùng, tính toán thực đơn theo độ tuổi, cân nặng và dị ứng gửi lên.
- **Body (JSON):**
  ```json
  {
    "age_months": 8,
    "weight_kg": 8.2,
    "allergies": ["Đậu phộng", "Tôm"]
  }
  ```
- **Kết quả:** Trả về 4 bữa ăn kèm tổng hàm lượng dinh dưỡng đạt mục tiêu khuyến nghị của WHO.

#### 2.2 Sinh thực đơn tuần theo tham số truyền vào (`POST /api/menu/weekly`)
- **Body (JSON):** Tương tự như trên, trả về thực đơn 7 ngày.

#### 2.3 Gợi ý thực đơn thai kỳ theo tuần (`GET /api/nutrition/meal-plan?week=16`)
- **Mô tả:** Thực đơn 7 ngày cho mẹ mang thai tuần 16 với đầy đủ Acid Folic, Sắt và Canxi.

---

### 3. Quản lý Hồ sơ Bé trên Microservice (`/api/baby`)

#### 3.1 Tạo hồ sơ bé trên Nutrition Service (`POST /api/baby/profile`)
- **Body (JSON):**
  ```json
  {
    "user_id": "user_guid_123",
    "baby_name": "Bé Miu",
    "date_of_birth": "2026-02-15",
    "gender": "Female",
    "current_weight_kg": 7.5,
    "current_height_cm": 66.0,
    "allergies": ["Trứng gà"]
  }
  ```
- **Hệ thống tự động tính:** `daily_calories_goal`, `daily_protein_goal`, `daily_iron_goal` theo chuẩn WHO.

#### 3.2 Cập nhật cân nặng / Chiều cao bé (`PATCH /api/baby/profile/{baby_id}`)
- **Mô tả:** Tự động tính lại các mục tiêu dinh dưỡng hàng ngày dựa trên cân nặng mới.

#### 3.3 Lấy thực đơn ngày theo Baby ID (`GET /api/baby/{baby_id}/menu/daily`)
#### 3.4 Lấy thực đơn tuần theo Baby ID (`GET /api/baby/{baby_id}/menu/weekly`)
#### 3.5 Xem mục tiêu dinh dưỡng khuyến nghị của bé (`GET /api/baby/{baby_id}/nutrition/goals`)

---

### 4. Quản lý Công thức Món ăn & Tự tính Dinh dưỡng (`/api/recipes`)

#### 4.1 Thêm công thức món ăn mới (`POST /api/recipes`)
- **Mô tả:** Nhập các nguyên liệu và khối lượng (gram). **Hệ thống tự động tra cứu USDA và cộng dồn chính xác** tổng Calories, Protein, Fat, Carbs, Sắt, Kẽm, Omega-3.
- **Body (JSON):**
  ```json
  {
    "name_vi": "Cháo thịt bò bí đỏ",
    "name_en": "Beef and Pumpkin Porridge",
    "description": "Món cháo thơm ngon giàu sắt cho bé bắt đầu tập ăn dặm.",
    "meal_type": "lunch",
    "texture": "puree",
    "min_age_months": 7,
    "max_age_months": 12,
    "servings": 1,
    "prep_time_min": 20,
    "ingredients": [
      { "name_en": "Beef", "weight_grams": 30.0 },
      { "name_en": "Pumpkin", "weight_grams": 40.0 },
      { "name_en": "Rice", "weight_grams": 35.0 }
    ],
    "cooking_steps": [
      { "step": 1, "instruction": "Băm nhuyễn thịt bò và ướp một chút dầu ô liu." },
      { "step": 2, "instruction": "Nấu nhừ cháo cùng bí đỏ nghiền rồi cho thịt bò vào khuấy đều." }
    ],
    "allergens": []
  }
  ```

#### 4.2 Lọc tìm kiếm công thức (`GET /api/recipes`)
- **Query Parameters:**
  - `age_months=7`: Lọc món ăn hợp lứa tuổi.
  - `texture=puree`: Lọc kết cấu (nhuyễn mịn, xay hạt, thô).
  - `allergen_exclude=tôm,trứng`: Lọc loại trừ món có chứa các dị ứng này.

#### 4.3 Tính toán lại tổng dinh dưỡng công thức (`POST /api/recipes/{recipe_id}/recalculate`)
- **Mô tả:** Chạy lại phép tính khi bảng dữ liệu thành phần dinh dưỡng nguyên liệu có sự cập nhật.

#### 4.4 Health Check (`GET /health`)
- **Mô tả:** Kiểm tra trạng thái sẵn sàng của FastAPI microservice. Trả về `{"status": "healthy", "service": "MomOi.NutritionAPI"}`.

---

## 🎯 BẢNG TÓM TẮT MÃ LỖI HTTP STATUS CODES

| Mã trạng thái | Ý nghĩa | Xử lý khuyến nghị cho Client |
| :--- | :--- | :--- |
| `200 OK` | Thành công | Đọc dữ liệu tại trường `data`. |
| `201 Created` | Tạo mới bản ghi thành công | Nhận ID và thông tin đối tượng mới tạo. |
| `400 Bad Request` | Dữ liệu gửi lên sai định dạng hoặc thiếu trường bắt buộc | Kiểm tra thông báo trong `message` hoặc mảng `errors`. |
| `401 Unauthorized` | Thiếu hoặc Token hết hạn | Gọi `POST /api/auth/refresh` hoặc chuyển về màn hình Đăng nhập. |
| `403 Forbidden` | Không có quyền truy cập (Role hoặc gói VIP không đủ) | Yêu cầu nâng cấp gói hoặc thông báo người dùng không đủ quyền. |
| `404 Not Found` | Không tìm thấy tài nguyên theo ID | Kiểm tra lại ID truyền vào. |
| `429 Too Many Requests`| Bị chặn do Rate Limit (> 100 requests/phút) | Chờ 1 phút trước khi gửi tiếp request mới. |
| `500 Internal Server Error`| Lỗi nội bộ hệ thống | Báo cáo bộ phận Backend kiểm tra log. |
