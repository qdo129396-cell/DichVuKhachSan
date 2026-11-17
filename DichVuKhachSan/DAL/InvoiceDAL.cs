using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class InvoiceDAL
    {
        public List<InvoiceDTO> GetAll()
        {
            return SearchAndFilter("", "", null, null);
        }

        public InvoiceDTO GetById(int invoiceId)
        {
            string query = @"SELECT i.InvoiceId, i.InvoiceCode, i.OrderId, i.IssuedBy, i.InvoiceDate,
                                    i.DiscountAmount, i.TotalAmount, i.PaymentStatus, i.PaymentMethod, i.PaidAt, i.Notes,
                                    o.OrderCode, o.RoomNumber,
                                    ISNULL(c.FullName, N'Khách vãng lai') AS CustomerName,
                                    u.FullName AS IssuedByName
                             FROM dbo.Invoices i
                             INNER JOIN dbo.ServiceOrders o ON i.OrderId = o.OrderId
                             LEFT JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                             INNER JOIN dbo.Users u ON i.IssuedBy = u.UserId
                             WHERE i.InvoiceId = @InvoiceId";

            SqlParameter[] parameters = {
                new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                var inv = MapRowToInvoice(dt.Rows[0]);
                inv.OrderDetails = new ServiceOrderDAL().GetDetailsByOrderId(inv.OrderId);
                return inv;
            }
            return null;
        }

        public List<InvoiceDTO> SearchAndFilter(string keyword, string status, DateTime? fromDate, DateTime? toDate)
        {
            List<InvoiceDTO> list = new List<InvoiceDTO>();
            string query = @"SELECT i.InvoiceId, i.InvoiceCode, i.OrderId, i.IssuedBy, i.InvoiceDate,
                                    i.DiscountAmount, i.TotalAmount, i.PaymentStatus, i.PaymentMethod, i.PaidAt, i.Notes,
                                    o.OrderCode, o.RoomNumber,
                                    ISNULL(c.FullName, N'Khách vãng lai') AS CustomerName,
                                    u.FullName AS IssuedByName
                             FROM dbo.Invoices i
                             INNER JOIN dbo.ServiceOrders o ON i.OrderId = o.OrderId
                             LEFT JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
                             INNER JOIN dbo.Users u ON i.IssuedBy = u.UserId
                             WHERE 1 = 1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query += " AND (i.InvoiceCode LIKE @Kw OR o.OrderCode LIKE @Kw OR c.FullName LIKE @Kw)";
                parameters.Add(new SqlParameter("@Kw", SqlDbType.NVarChar, 100) { Value = "%" + keyword.Trim() + "%" });
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "Tất cả")
            {
                query += " AND i.PaymentStatus = @Status";
                parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 20) { Value = status });
            }

            if (fromDate.HasValue)
            {
                query += " AND i.InvoiceDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime2) { Value = fromDate.Value.Date });
            }

            if (toDate.HasValue)
            {
                query += " AND i.InvoiceDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime2) { Value = toDate.Value.Date.AddDays(1).AddTicks(-1) });
            }

            query += " ORDER BY i.InvoiceId DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapRowToInvoice(row));
            }
            return list;
        }

        public bool ProcessPayment(int invoiceId, decimal discountAmount, string paymentMethod, string notes)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Check if already paid
                        string checkSql = "SELECT PaymentStatus, OrderId FROM dbo.Invoices WHERE InvoiceId = @InvoiceId";
                        string currentStatus = "";
                        int orderId = 0;
                        using (var cmdCheck = new SqlCommand(checkSql, conn, trans))
                        {
                            cmdCheck.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
                            using (var rdr = cmdCheck.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    currentStatus = rdr["PaymentStatus"].ToString();
                                    orderId = Convert.ToInt32(rdr["OrderId"]);
                                }
                            }
                        }

                        if (currentStatus == "Paid")
                        {
                            throw new InvalidOperationException("Hóa đơn này đã được thanh toán trước đó, không thể thanh toán lại!");
                        }

                        // Update Invoice
                        string updateInvoiceSql = @"UPDATE dbo.Invoices
                                                   SET PaymentStatus = N'Paid',
                                                       PaymentMethod = @Method,
                                                       DiscountAmount = @Discount,
                                                       PaidAt = SYSDATETIME(),
                                                       Notes = @Notes
                                                   WHERE InvoiceId = @InvoiceId";

                        using (var cmdUpdate = new SqlCommand(updateInvoiceSql, conn, trans))
                        {
                            cmdUpdate.Parameters.Add(new SqlParameter("@Method", SqlDbType.NVarChar, 30) { Value = paymentMethod });
                            cmdUpdate.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Decimal) { Value = discountAmount });
                            cmdUpdate.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)notes ?? DBNull.Value });
                            cmdUpdate.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });

                            cmdUpdate.ExecuteNonQuery();
                        }

                        // Update Order status to Completed
                        string updateOrderSql = "UPDATE dbo.ServiceOrders SET Status = N'Completed' WHERE OrderId = @OrderId";
                        using (var cmdOrder = new SqlCommand(updateOrderSql, conn, trans))
                        {
                            cmdOrder.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId });
                            cmdOrder.ExecuteNonQuery();
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

        public bool CancelInvoice(int invoiceId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string checkSql = "SELECT PaymentStatus, OrderId FROM dbo.Invoices WHERE InvoiceId = @InvoiceId";
                        string currentStatus = "";
                        int orderId = 0;
                        using (var cmdCheck = new SqlCommand(checkSql, conn, trans))
                        {
                            cmdCheck.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
                            using (var rdr = cmdCheck.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    currentStatus = rdr["PaymentStatus"].ToString();
                                    orderId = Convert.ToInt32(rdr["OrderId"]);
                                }
                            }
                        }

                        if (currentStatus == "Paid")
                        {
                            throw new InvalidOperationException("Không thể hủy hóa đơn đã thanh toán theo quy tắc nghiệp vụ!");
                        }

                        string sqlInv = "UPDATE dbo.Invoices SET PaymentStatus = N'Cancelled' WHERE InvoiceId = @InvoiceId";
                        using (var cmd = new SqlCommand(sqlInv, conn, trans))
                        {
                            cmd.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
                            cmd.ExecuteNonQuery();
                        }

                        string sqlOrd = "UPDATE dbo.ServiceOrders SET Status = N'Cancelled' WHERE OrderId = @OrderId";
                        using (var cmd = new SqlCommand(sqlOrd, conn, trans))
                        {
                            cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId });
                            cmd.ExecuteNonQuery();
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

        private InvoiceDTO MapRowToInvoice(DataRow row)
        {
            return new InvoiceDTO
            {
                InvoiceId = Convert.ToInt32(row["InvoiceId"]),
                InvoiceCode = row["InvoiceCode"].ToString(),
                OrderId = Convert.ToInt32(row["OrderId"]),
                OrderCode = row["OrderCode"].ToString(),
                CustomerName = row["CustomerName"].ToString(),
                RoomNumber = row["RoomNumber"] == DBNull.Value ? "" : row["RoomNumber"].ToString(),
                IssuedBy = Convert.ToInt32(row["IssuedBy"]),
                IssuedByName = row["IssuedByName"].ToString(),
                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                PaymentStatus = row["PaymentStatus"].ToString(),
                PaymentMethod = row["PaymentMethod"] == DBNull.Value ? "Chưa thanh toán" : row["PaymentMethod"].ToString(),
                PaidAt = row["PaidAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["PaidAt"]),
                Notes = row["Notes"] == DBNull.Value ? "" : row["Notes"].ToString()
            };
        }
    }
}
