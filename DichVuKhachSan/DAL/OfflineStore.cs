using System;
using System.Collections.Generic;
using System.Linq;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    // Session-only data used when SQL Server is unavailable. It is deliberately
    // never written to disk or synchronized back to SQL Server.
    internal static class OfflineStore
    {
        internal static readonly List<UserDTO> Users = new List<UserDTO>();
        internal static readonly List<CustomerDTO> Customers = new List<CustomerDTO>();
        internal static readonly List<ServiceDTO> Services = new List<ServiceDTO>();
        internal static readonly List<ServiceOrderDTO> Orders = new List<ServiceOrderDTO>();
        internal static readonly List<InvoiceDTO> Invoices = new List<InvoiceDTO>();

        static OfflineStore()
        {
            Services.AddRange(new[] {
                new ServiceDTO { ServiceId = 1, ServiceCode = "DV001", ServiceName = "Giặt ủi quần áo", Category = "Giặt ủi", Unit = "Kg", UnitPrice = 30000 },
                new ServiceDTO { ServiceId = 2, ServiceCode = "DV002", ServiceName = "Nước suối", Category = "Đồ uống", Unit = "Chai", UnitPrice = 15000 },
                new ServiceDTO { ServiceId = 3, ServiceCode = "DV003", ServiceName = "Massage thư giãn", Category = "Spa", Unit = "Giờ", UnitPrice = 250000 },
                new ServiceDTO { ServiceId = 4, ServiceCode = "DV004", ServiceName = "Xông hơi", Category = "Spa", Unit = "Lượt", UnitPrice = 180000 }
            });
            Customers.Add(new CustomerDTO { CustomerId = 1, CustomerCode = "KH001", FullName = "Khách hàng mẫu", Phone = "0900000000" });
        }

        internal static int NextId<T>(IEnumerable<T> items, Func<T, int> selector) => items.Any() ? items.Max(selector) + 1 : 1;

        internal static bool CreateOrder(ServiceOrderDTO order, List<ServiceOrderDetailDTO> details, out int orderId, out string invoiceCode)
        {
            int newOrderId = NextId(Orders, x => x.OrderId);
            orderId = newOrderId;
            order.OrderId = newOrderId;
            order.OrderCode ??= $"DV-{DateTime.Now:yyMMdd}-{newOrderId:D3}";
            order.CreatedByName = Users.FirstOrDefault(x => x.UserId == order.CreatedBy)?.FullName ?? "Người dùng ngoại tuyến";
            var customer = Customers.FirstOrDefault(x => x.CustomerId == order.CustomerId);
            order.CustomerName = customer?.FullName ?? "Khách vãng lai";
            order.CustomerPhone = customer?.Phone ?? "";
            order.TotalAmount = details.Sum(x => x.Quantity * x.UnitPrice);
            order.Details = details.Select((x, i) => new ServiceOrderDetailDTO {
                OrderDetailId = i + 1, OrderId = newOrderId, ServiceId = x.ServiceId,
                ServiceCode = Services.FirstOrDefault(s => s.ServiceId == x.ServiceId)?.ServiceCode ?? x.ServiceCode,
                ServiceName = x.ServiceName, Quantity = x.Quantity, UnitPrice = x.UnitPrice, Notes = x.Notes
            }).ToList();
            Orders.Add(order);
            int invoiceId = NextId(Invoices, x => x.InvoiceId);
            invoiceCode = $"HD{DateTime.Now:yyMMdd}-{invoiceId:D3}";
            Invoices.Add(new InvoiceDTO { InvoiceId = invoiceId, InvoiceCode = invoiceCode, OrderId = newOrderId,
                OrderCode = order.OrderCode, CustomerName = order.CustomerName, RoomNumber = order.RoomNumber,
                IssuedBy = order.CreatedBy, IssuedByName = order.CreatedByName, InvoiceDate = DateTime.Now,
                TotalAmount = order.TotalAmount, PaymentStatus = "Unpaid", Notes = order.Notes, OrderDetails = order.Details });
            return true;
        }
    }
}
