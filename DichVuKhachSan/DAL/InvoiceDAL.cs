using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            if (DatabaseHelper.IsOfflineMode) return OfflineStore.Invoices.FirstOrDefault(x => x.InvoiceId == invoiceId);
            string query = @"SELECT i.InvoiceId, i.InvoiceCode, i.OrderId, o.CustomerId, i.IssuedBy, i.InvoiceDate,
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
            if (DatabaseHelper.IsOfflineMode)
                return OfflineStore.Invoices.Where(x => (string.IsNullOrWhiteSpace(keyword) || ($"{x.InvoiceCode} {x.OrderCode} {x.CustomerName}").IndexOf(keyword.Trim(), StringComparison.OrdinalIgnoreCase) >= 0) && (string.IsNullOrWhiteSpace(status) || status == "Tất cả" || x.PaymentStatus == status) && (!fromDate.HasValue || x.InvoiceDate.Date >= fromDate.Value.Date) && (!toDate.HasValue || x.InvoiceDate.Date <= toDate.Value.Date)).OrderByDescending(x => x.InvoiceId).ToList();
            List<InvoiceDTO> list = new List<InvoiceDTO>();
            string query = @"SELECT i.InvoiceId, i.InvoiceCode, i.OrderId, o.CustomerId, i.IssuedBy, i.InvoiceDate,
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
            if (DatabaseHelper.IsOfflineMode) { var invoice = OfflineStore.Invoices.FirstOrDefault(x => x.InvoiceId == invoiceId); if (invoice == null) return false; if (invoice.PaymentStatus == "Paid") throw new InvalidOperationException("Hóa đơn này đã được thanh toán trước đó, không thể thanh toán lại!"); invoice.PaymentStatus = "Paid"; invoice.PaymentMethod = paymentMethod; invoice.DiscountAmount = discountAmount; invoice.PaidAt = DateTime.Now; invoice.Notes = notes; var order = OfflineStore.Orders.FirstOrDefault(x => x.OrderId == invoice.OrderId); if (order != null) order.Status = "Completed"; return true; }
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

        public bool ProcessPayments(IEnumerable<int> invoiceIds, decimal discountAmount, string paymentMethod, string notes)
        {
            var ids = invoiceIds.Distinct().ToList();
            if (ids.Count == 0) return false;
            if (DatabaseHelper.IsOfflineMode)
            {
                var invoices = OfflineStore.Invoices.Where(x => ids.Contains(x.InvoiceId)).ToList();
                if (invoices.Count != ids.Count || invoices.Any(x => x.PaymentStatus != "Unpaid"))
                    throw new InvalidOperationException("Một hoặc nhiều hóa đơn không còn ở trạng thái chưa thanh toán.");
                decimal remainingDiscount = discountAmount;
                foreach (var invoice in invoices)
                {
                    var applied = Math.Min(remainingDiscount, invoice.TotalAmount);
                    remainingDiscount -= applied;
                    invoice.PaymentStatus = "Paid"; invoice.PaymentMethod = paymentMethod; invoice.DiscountAmount = applied; invoice.PaidAt = DateTime.Now; invoice.Notes = notes;
                    var order = OfflineStore.Orders.FirstOrDefault(x => x.OrderId == invoice.OrderId); if (order != null) order.Status = "Completed";
                }
                return true;
            }
            using var conn = DatabaseHelper.GetConnection(); conn.Open(); using var trans = conn.BeginTransaction();
            try
            {
                decimal remainingDiscount = discountAmount;
                foreach (var id in ids)
                {
                    decimal total; string status; int orderId;
                    using (var check = new SqlCommand("SELECT TotalAmount, PaymentStatus, OrderId FROM dbo.Invoices WHERE InvoiceId=@Id", conn, trans))
                    { check.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id }); using var r = check.ExecuteReader(); if (!r.Read()) throw new InvalidOperationException("Không tìm thấy hóa đơn."); total = Convert.ToDecimal(r["TotalAmount"]); status = r["PaymentStatus"].ToString(); orderId = Convert.ToInt32(r["OrderId"]); }
                    if (status != "Unpaid") throw new InvalidOperationException("Một hoặc nhiều hóa đơn không còn ở trạng thái chưa thanh toán.");
                    var applied = Math.Min(remainingDiscount, total); remainingDiscount -= applied;
                    using (var update = new SqlCommand("UPDATE dbo.Invoices SET PaymentStatus=N'Paid', PaymentMethod=@Method, DiscountAmount=@Discount, PaidAt=SYSDATETIME(), Notes=@Notes WHERE InvoiceId=@Id; UPDATE dbo.ServiceOrders SET Status=N'Completed' WHERE OrderId=@OrderId;", conn, trans))
                    { update.Parameters.Add(new SqlParameter("@Method", SqlDbType.NVarChar, 30) { Value = paymentMethod }); update.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Decimal) { Value = applied }); update.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Value = (object)notes ?? DBNull.Value }); update.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id }); update.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }); update.ExecuteNonQuery(); }
                }
                trans.Commit(); return true;
            }
            catch { trans.Rollback(); throw; }
        }

        public bool CancelInvoice(int invoiceId)
        {
            if (DatabaseHelper.IsOfflineMode) { var invoice = OfflineStore.Invoices.FirstOrDefault(x => x.InvoiceId == invoiceId); if (invoice == null) return false; if (invoice.PaymentStatus == "Paid") throw new InvalidOperationException("Không thể hủy hóa đơn đã thanh toán theo quy tắc nghiệp vụ!"); invoice.PaymentStatus = "Cancelled"; var order = OfflineStore.Orders.FirstOrDefault(x => x.OrderId == invoice.OrderId); if (order != null) order.Status = "Cancelled"; return true; }
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
                CustomerId = row["CustomerId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CustomerId"]),
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
