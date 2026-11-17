using System;
using System.Collections.Generic;

namespace DichVuKhachSan.DTO
{
    public class ServiceOrderDTO
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string RoomNumber { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled
        public string Notes { get; set; }
        public decimal TotalAmount { get; set; }

        public List<ServiceOrderDetailDTO> Details { get; set; } = new List<ServiceOrderDetailDTO>();
    }

    public class ServiceOrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public decimal Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
        public string Notes { get; set; }
    }
}
