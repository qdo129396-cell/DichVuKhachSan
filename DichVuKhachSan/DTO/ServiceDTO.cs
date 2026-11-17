using System;

namespace DichVuKhachSan.DTO
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; } = "Lần";
        public decimal UnitPrice { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
