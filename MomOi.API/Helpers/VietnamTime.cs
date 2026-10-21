using System;

namespace MomOi.API.Helpers
{
    /// <summary>
    /// Giờ Việt Nam (UTC+7, không có giờ mùa hè).
    /// App lưu giờ nhắc theo giờ VN ("07:00") và ngày lịch dưới dạng 00:00 UTC của ngày đó,
    /// nên mọi phép so "bây giờ" / "hôm nay" phải quy về giờ VN trước.
    /// Dùng offset cố định thay vì TimeZoneInfo để không phụ thuộc tzdata của container.
    /// </summary>
    public static class VietnamTime
    {
        private static readonly TimeSpan Offset = TimeSpan.FromHours(7);

        /// <summary>Giờ hiện tại theo giờ VN.</summary>
        public static DateTime Now => DateTime.UtcNow.Add(Offset);

        /// <summary>
        /// Ngày hôm nay theo giờ VN, biểu diễn là 00:00 UTC của ngày đó
        /// (cùng quy ước với ngày app gửi lên và ngày đã lưu trong DB).
        /// </summary>
        public static DateTime Today => DateTime.SpecifyKind(Now.Date, DateTimeKind.Utc);
    }
}
