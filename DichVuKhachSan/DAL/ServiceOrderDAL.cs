using System;
using System.Collections.Generic;
using System.Data;
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
            string query = "UPDATE dbo.ServiceOrders SET Status = @Status WHERE OrderId = @OrderId";
            SqlParameter[] parameters = {
                new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status },
                new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }
            };
            return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public string GenerateNextOrderCode()
        {
            string datePart = DateTime.Now.ToString("yyMMdd");
            string query = @"SELECT ISNULL(MAX(OrderId), 0) + 1 FROM dbo.ServiceOrders";
            int nextId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
            return $"DV-{datePart}-{nextId:D3}";
        }
    }
}
