using System;
using System.Collections.Generic;

namespace DichVuKhachSan.DTO
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string OrderCode { get; set; }
        public string CustomerName { get; set; }
        public string RoomNumber { get; set; }
        public int IssuedBy { get; set; }
        public string IssuedByName { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount => Math.Max(0, TotalAmount - DiscountAmount);
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, Cancelled
        public string PaymentMethod { get; set; } // Tiền mặt, Chuyển khoản
        public DateTime? PaidAt { get; set; }
        public string Notes { get; set; }

        public List<ServiceOrderDetailDTO> OrderDetails { get; set; } = new List<ServiceOrderDetailDTO>();
    }
}
