using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class ServiceOrderDAL
    {
        public bool CreateOrderWithDetails(ServiceOrderDTO order, List<ServiceOrderDetailDTO> details, out int createdOrderId, out string createdInvoiceCode)
        {
            createdOrderId = 0;
            createdInvoiceCode = string.Empty;
            if (DatabaseHelper.IsOfflineMode)
                return OfflineStore.CreateOrder(order, details, out createdOrderId, out createdInvoiceCode);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert Service Order
                        string sqlOrder = @"INSERT INTO dbo.ServiceOrders (OrderCode, CustomerId, RoomNumber, CreatedBy, OrderDate, Status, Notes)
                                           VALUES (@OrderCode, @CustomerId, @RoomNumber, @CreatedBy, @OrderDate, @Status, @Notes);
                                           SELECT SCOPE_IDENTITY();";

                        using (var cmdOrder = new SqlCommand(sqlOrder, conn, trans))
                        {
                            cmdOrder.Parameters.Add(new SqlParameter("@OrderCode", SqlDbType.NVarChar, 30) { Value = order.OrderCode });
                            cmdOrder.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = (object)order.CustomerId ?? DBNull.Value });
                            cmdOrder.Parameters.Add(new SqlParameter("@RoomNumber", SqlDbType.NVarChar, 20) { Value = (object)order.RoomNumber ?? DBNull.Value });
                            cmdOrder.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = order.CreatedBy });
                            cmdOrder.Parameters.Add(new SqlParameter("@OrderDate", SqlDbType.DateTime2) { Value = order.OrderDate });
                            cmdOrder.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = order.Status });
                            cmdOrder.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)order.Notes ?? DBNull.Value });

                            createdOrderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                        }

                        // 2. Insert Service Order Details & calculate total
                        decimal totalAmount = 0;
                        string sqlDetail = @"INSERT INTO dbo.ServiceOrderDetails (OrderId, ServiceId, Quantity, UnitPrice, Notes)
                                            VALUES (@OrderId, @ServiceId, @Quantity, @UnitPrice, @Notes);";

                        foreach (var item in details)
                        {
                            using (var cmdDetail = new SqlCommand(sqlDetail, conn, trans))
                            {
                                cmdDetail.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = createdOrderId });
                                cmdDetail.Parameters.Add(new SqlParameter("@ServiceId", SqlDbType.Int) { Value = item.ServiceId });
                                cmdDetail.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal) { Value = item.Quantity });
                                cmdDetail.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Value = item.UnitPrice });
                                cmdDetail.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 250) { Value = (object)item.Notes ?? DBNull.Value });

                                cmdDetail.ExecuteNonQuery();
                            }
                            totalAmount += item.Quantity * item.UnitPrice;
                        }

                        // 3. Automatically create Invoice for this order
                        createdInvoiceCode = $"HD{DateTime.Now:yyMMdd}-{createdOrderId:D3}";
                        string sqlInvoice = @"INSERT INTO dbo.Invoices (InvoiceCode, OrderId, IssuedBy, InvoiceDate, DiscountAmount, TotalAmount, PaymentStatus, Notes)
                                             VALUES (@InvoiceCode, @OrderId, @IssuedBy, @InvoiceDate, 0, @TotalAmount, N'Unpaid', @Notes);";

                        using (var cmdInv = new SqlCommand(sqlInvoice, conn, trans))
                        {
                            cmdInv.Parameters.Add(new SqlParameter("@InvoiceCode", SqlDbType.NVarChar, 30) { Value = createdInvoiceCode });
                            cmdInv.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = createdOrderId });
                            cmdInv.Parameters.Add(new SqlParameter("@IssuedBy", SqlDbType.Int) { Value = order.CreatedBy });
                            cmdInv.Parameters.Add(new SqlParameter("@InvoiceDate", SqlDbType.DateTime2) { Value = DateTime.Now });
                            cmdInv.Parameters.Add(new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Value = totalAmount });
                            cmdInv.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)order.Notes ?? DBNull.Value });

                            cmdInv.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<ServiceOrderDTO> GetAllOrders()
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Orders.OrderByDescending(x => x.OrderId).ToList();
            List<ServiceOrderDTO> list = new List<ServiceOrderDTO>();
            string query = @"SELECT o.OrderId, o.OrderCode, o.CustomerId, o.RoomNumber, o.CreatedBy, o.OrderDate, o.Status, o.Notes,
                                    ISNULL(c.FullName, N'Khách vãng lai') AS CustomerName,
                                    c.Phone AS CustomerPhone,
                                    u.FullName AS CreatedByName,
                                    ISNULL(SUM(d.Quantity * d.UnitPrice), 0) AS TotalAmount
                             FROM dbo.ServiceOrders o
                             LEFT JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                             INNER JOIN dbo.Users u ON o.CreatedBy = u.UserId
                             LEFT JOIN dbo.ServiceOrderDetails d ON o.OrderId = d.OrderId
                             GROUP BY o.OrderId, o.OrderCode, o.CustomerId, o.RoomNumber, o.CreatedBy, o.OrderDate, o.Status, o.Notes,
                                      c.FullName, c.Phone, u.FullName
                             ORDER BY o.OrderId DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ServiceOrderDTO
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    OrderCode = row["OrderCode"].ToString(),
                    CustomerId = row["CustomerId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CustomerId"]),
                    CustomerName = row["CustomerName"].ToString(),
                    CustomerPhone = row["CustomerPhone"] == DBNull.Value ? "" : row["CustomerPhone"].ToString(),
                    RoomNumber = row["RoomNumber"] == DBNull.Value ? "" : row["RoomNumber"].ToString(),
                    CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                    CreatedByName = row["CreatedByName"].ToString(),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    Status = row["Status"].ToString(),
                    Notes = row["Notes"] == DBNull.Value ? "" : row["Notes"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"])
                });
            }
            return list;
        }

        public List<ServiceOrderDetailDTO> GetDetailsByOrderId(int orderId)
        {
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Orders.FirstOrDefault(x => x.OrderId == orderId)?.Details ?? new List<ServiceOrderDetailDTO>();
            List<ServiceOrderDetailDTO> list = new List<ServiceOrderDetailDTO>();
            string query = @"SELECT d.OrderDetailId, d.OrderId, d.ServiceId, d.Quantity, d.UnitPrice,
                                    s.ServiceCode, s.ServiceName, d.Notes
                             FROM dbo.ServiceOrderDetails d
                             INNER JOIN dbo.Services s ON d.ServiceId = s.ServiceId
                             WHERE d.OrderId = @OrderId";

            SqlParameter[] parameters = {
                new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ServiceOrderDetailDTO
                {
                    OrderDetailId = Convert.ToInt32(row["OrderDetailId"]),
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    ServiceId = Convert.ToInt32(row["ServiceId"]),
                    ServiceCode = row["ServiceCode"].ToString(),
                    ServiceName = row["ServiceName"].ToString(),
                    Quantity = Convert.ToDecimal(row["Quantity"]),
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    Notes = row["Notes"] == DBNull.Value ? "" : row["Notes"].ToString()
                });
            }
            return list;
        }

        public bool UpdateStatus(int orderId, string status)
        {
            if (DatabaseHelper.IsOfflineMode) { var order = OfflineStore.Orders.FirstOrDefault(x => x.OrderId == orderId); if (order == null) return false; order.Status = status; return true; }
            string query = "UPDATE dbo.ServiceOrders SET Status = @Status WHERE OrderId = @OrderId";
            SqlParameter[] parameters = {
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status },
                new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateDetailQuantity(int orderDetailId, decimal quantity)
        {
            if (quantity <= 0) return false;
            if (DatabaseHelper.IsOfflineMode)
            {
                var order = OfflineStore.Orders.FirstOrDefault(x => x.Details.Any(d => d.OrderDetailId == orderDetailId));
                var detail = order?.Details.FirstOrDefault(x => x.OrderDetailId == orderDetailId); if (detail == null || order.Status == "Completed") return false;
                detail.Quantity = quantity; order.TotalAmount = order.Details.Sum(x => x.LineTotal);
                var invoice = OfflineStore.Invoices.FirstOrDefault(x => x.OrderId == order.OrderId); if (invoice != null && invoice.PaymentStatus == "Unpaid") { invoice.TotalAmount = order.TotalAmount; invoice.OrderDetails = order.Details; }
                return true;
            }
            using var conn = DatabaseHelper.GetConnection(); conn.Open(); using var trans = conn.BeginTransaction();
            try
            {
                int orderId;
                using (var cmd = new SqlCommand("SELECT d.OrderId FROM dbo.ServiceOrderDetails d INNER JOIN dbo.Invoices i ON i.OrderId=d.OrderId WHERE d.OrderDetailId=@Id AND i.PaymentStatus=N'Unpaid'", conn, trans)) { cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = orderDetailId }); var value = cmd.ExecuteScalar(); if (value == null) return false; orderId = Convert.ToInt32(value); }
                using (var cmd = new SqlCommand("UPDATE dbo.ServiceOrderDetails SET Quantity=@Quantity WHERE OrderDetailId=@Id; UPDATE dbo.Invoices SET TotalAmount=(SELECT ISNULL(SUM(Quantity*UnitPrice),0) FROM dbo.ServiceOrderDetails WHERE OrderId=@OrderId) WHERE OrderId=@OrderId AND PaymentStatus=N'Unpaid';", conn, trans)) { cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal) { Value = quantity }); cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = orderDetailId }); cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }); cmd.ExecuteNonQuery(); }
                trans.Commit(); return true;
            }
            catch { trans.Rollback(); throw; }
        }

        public bool UpdateDetail(ServiceOrderDetailDTO detail)
        {
            if (detail == null || detail.Quantity <= 0) return false;
            if (DatabaseHelper.IsOfflineMode)
            {
                var order = OfflineStore.Orders.FirstOrDefault(x => x.Details.Any(d => d.OrderDetailId == detail.OrderDetailId));
                var old = order?.Details.FirstOrDefault(x => x.OrderDetailId == detail.OrderDetailId); if (old == null || order.Status == "Completed") return false;
                old.ServiceId = detail.ServiceId; old.ServiceName = detail.ServiceName; old.UnitPrice = detail.UnitPrice; old.Quantity = detail.Quantity; old.Notes = detail.Notes;
                order.TotalAmount = order.Details.Sum(x => x.LineTotal); var invoice = OfflineStore.Invoices.FirstOrDefault(x => x.OrderId == order.OrderId); if (invoice != null && invoice.PaymentStatus == "Unpaid") invoice.TotalAmount = order.TotalAmount;
                return true;
            }
            using var conn = DatabaseHelper.GetConnection(); conn.Open(); using var trans = conn.BeginTransaction();
            try
            {
                int orderId;
                using (var cmd = new SqlCommand("SELECT d.OrderId FROM dbo.ServiceOrderDetails d INNER JOIN dbo.Invoices i ON i.OrderId=d.OrderId WHERE d.OrderDetailId=@Id AND i.PaymentStatus=N'Unpaid'", conn, trans)) { cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = detail.OrderDetailId }); var value = cmd.ExecuteScalar(); if (value == null) return false; orderId = Convert.ToInt32(value); }
                using (var cmd = new SqlCommand("UPDATE dbo.ServiceOrderDetails SET ServiceId=@ServiceId, Quantity=@Quantity, UnitPrice=@UnitPrice, Notes=@Notes WHERE OrderDetailId=@Id; UPDATE dbo.Invoices SET TotalAmount=(SELECT ISNULL(SUM(Quantity*UnitPrice),0) FROM dbo.ServiceOrderDetails WHERE OrderId=@OrderId) WHERE OrderId=@OrderId AND PaymentStatus=N'Unpaid';", conn, trans))
                { cmd.Parameters.Add(new SqlParameter("@ServiceId", SqlDbType.Int) { Value = detail.ServiceId }); cmd.Parameters.Add(new SqlParameter("@Quantity", SqlDbType.Decimal) { Value = detail.Quantity }); cmd.Parameters.Add(new SqlParameter("@UnitPrice", SqlDbType.Decimal) { Value = detail.UnitPrice }); cmd.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 250) { Value = (object)detail.Notes ?? DBNull.Value }); cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = detail.OrderDetailId }); cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }); cmd.ExecuteNonQuery(); }
                trans.Commit(); return true;
            }
            catch { trans.Rollback(); throw; }
        }

        public string GenerateNextOrderCode()
        {
            if (DatabaseHelper.IsOfflineMode) return $"DV-{DateTime.Now:yyMMdd}-{OfflineStore.NextId(OfflineStore.Orders, x => x.OrderId):D3}";
            string datePart = DateTime.Now.ToString("yyMMdd");
            string query = @"SELECT ISNULL(MAX(OrderId), 0) + 1 FROM dbo.ServiceOrders";
            int nextId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
            return $"DV-{datePart}-{nextId:D3}";
        }
    }
}
