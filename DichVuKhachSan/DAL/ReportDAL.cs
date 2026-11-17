using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.DAL
{
    public class ReportDAL
    {
        public DashboardStatsDTO GetDashboardStats()
        {
            var stats = new DashboardStatsDTO();

            // Total Customers
            string sqlCust = "SELECT COUNT(*) FROM dbo.Customers WHERE IsActive = 1";
            stats.TotalCustomers = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlCust));

            // Total Active Services
            string sqlServ = "SELECT COUNT(*) FROM dbo.Services WHERE IsActive = 1";
            stats.TotalServices = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlServ));

            // Total Invoices
            string sqlInv = "SELECT COUNT(*) FROM dbo.Invoices";
            stats.TotalInvoices = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sqlInv));

            // Today Revenue (paid invoices only)
            string sqlTodayRev = @"SELECT ISNULL(SUM(TotalAmount - DiscountAmount), 0)
                                   FROM dbo.Invoices
                                   WHERE PaymentStatus = N'Paid'
                                     AND CAST(InvoiceDate AS DATE) = CAST(SYSDATETIME() AS DATE)";
            stats.TodayRevenue = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(sqlTodayRev));

            // Revenue last 7 days
            stats.Revenue7Days = GetRevenueLast7Days();

            // Recent activities
            stats.RecentActivities = GetRecentActivities(4);

            return stats;
        }

        public List<DailyRevenueDTO> GetRevenueLast7Days()
        {
            List<DailyRevenueDTO> list = new List<DailyRevenueDTO>();
            DateTime today = DateTime.Today;

            for (int i = 6; i >= 0; i--)
            {
                DateTime day = today.AddDays(-i);
                string dayLabel = GetDayOfWeekLabel(day.DayOfWeek);

                string sql = @"SELECT ISNULL(COUNT(*), 0) AS Cnt,
                                      ISNULL(SUM(TotalAmount - DiscountAmount), 0) AS Rev
                               FROM dbo.Invoices
                               WHERE PaymentStatus = N'Paid'
                                 AND CAST(InvoiceDate AS DATE) = @Day";

                SqlParameter[] parameters = {
                    new SqlParameter("@Day", SqlDbType.Date) { Value = day }
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
                decimal rev = 0;
                int count = 0;
                if (dt.Rows.Count > 0)
                {
                    count = Convert.ToInt32(dt.Rows[0]["Cnt"]);
                    rev = Convert.ToDecimal(dt.Rows[0]["Rev"]);
                }

                list.Add(new DailyRevenueDTO
                {
                    Date = day,
                    DayLabel = dayLabel,
                    InvoiceCount = count,
                    Revenue = rev
                });
            }

            return list;
        }

        public List<DailyRevenueDTO> GetDailyRevenueRange(DateTime fromDate, DateTime toDate)
        {
            List<DailyRevenueDTO> list = new List<DailyRevenueDTO>();
            string sql = @"SELECT CAST(InvoiceDate AS DATE) AS RevDate,
                                  COUNT(*) AS InvoiceCount,
                                  ISNULL(SUM(TotalAmount - DiscountAmount), 0) AS Revenue
                           FROM dbo.Invoices
                           WHERE PaymentStatus = N'Paid'
                             AND CAST(InvoiceDate AS DATE) >= @FromDate
                             AND CAST(InvoiceDate AS DATE) <= @ToDate
                           GROUP BY CAST(InvoiceDate AS DATE)
                           ORDER BY RevDate ASC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDate.Date }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            foreach (DataRow row in dt.Rows)
            {
                DateTime d = Convert.ToDateTime(row["RevDate"]);
                list.Add(new DailyRevenueDTO
                {
                    Date = d,
                    DayLabel = GetDayOfWeekLabel(d.DayOfWeek),
                    InvoiceCount = Convert.ToInt32(row["InvoiceCount"]),
                    Revenue = Convert.ToDecimal(row["Revenue"])
                });
            }

            return list;
        }

        public List<CategoryRevenueDTO> GetCategoryRevenueRange(DateTime fromDate, DateTime toDate)
        {
            List<CategoryRevenueDTO> list = new List<CategoryRevenueDTO>();
            string sql = @"SELECT s.Category,
                                  ISNULL(SUM(d.Quantity * d.UnitPrice), 0) AS Revenue
                           FROM dbo.Invoices i
                           INNER JOIN dbo.ServiceOrders o ON i.OrderId = o.OrderId
                           INNER JOIN dbo.ServiceOrderDetails d ON o.OrderId = d.OrderId
                           INNER JOIN dbo.Services s ON d.ServiceId = s.ServiceId
                           WHERE i.PaymentStatus = N'Paid'
                             AND CAST(i.InvoiceDate AS DATE) >= @FromDate
                             AND CAST(i.InvoiceDate AS DATE) <= @ToDate
                           GROUP BY s.Category
                           ORDER BY Revenue DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Date },
                new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDate.Date }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters);
            decimal totalAll = 0;
            foreach (DataRow row in dt.Rows)
            {
                totalAll += Convert.ToDecimal(row["Revenue"]);
            }

            foreach (DataRow row in dt.Rows)
            {
                decimal rev = Convert.ToDecimal(row["Revenue"]);
                float pct = totalAll > 0 ? (float)((rev / totalAll) * 100) : 0f;
                list.Add(new CategoryRevenueDTO
                {
                    Category = row["Category"].ToString(),
                    Revenue = rev,
                    Percentage = pct
                });
            }

            return list;
        }

        public void GetInvoiceCounts(DateTime? fromDate, DateTime? toDate, out int totalCount, out int paidCount, out int unpaidCount, out decimal totalRevenue)
        {
            string sql = @"SELECT 
                                COUNT(*) AS TotalCount,
                                ISNULL(SUM(CASE WHEN PaymentStatus = N'Paid' THEN 1 ELSE 0 END), 0) AS PaidCount,
                                ISNULL(SUM(CASE WHEN PaymentStatus = N'Unpaid' THEN 1 ELSE 0 END), 0) AS UnpaidCount,
                                ISNULL(SUM(CASE WHEN PaymentStatus = N'Paid' THEN (TotalAmount - DiscountAmount) ELSE 0 END), 0) AS TotalRevenue
                           FROM dbo.Invoices
                           WHERE 1 = 1";

            List<SqlParameter> parameters = new List<SqlParameter>();
            if (fromDate.HasValue)
            {
                sql += " AND CAST(InvoiceDate AS DATE) >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.Date) { Value = fromDate.Value.Date });
            }
            if (toDate.HasValue)
            {
                sql += " AND CAST(InvoiceDate AS DATE) <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.Date) { Value = toDate.Value.Date });
            }

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters.ToArray());
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                totalCount = Convert.ToInt32(r["TotalCount"]);
                paidCount = Convert.ToInt32(r["PaidCount"]);
                unpaidCount = Convert.ToInt32(r["UnpaidCount"]);
                totalRevenue = Convert.ToDecimal(r["TotalRevenue"]);
            }
            else
            {
                totalCount = 0;
                paidCount = 0;
                unpaidCount = 0;
                totalRevenue = 0;
            }
        }

        public List<RecentActivityDTO> GetRecentActivities(int top = 5)
        {
            List<RecentActivityDTO> list = new List<RecentActivityDTO>();

            // Query recent orders and invoices
            string sql = $@"SELECT TOP ({top}) * FROM (
                                SELECT N'Đặt dịch vụ - ' + ISNULL(c.FullName, N'Khách vãng lai') AS Title,
                                       o.OrderDate AS ActivityTime,
                                       N'Đặt dịch vụ' AS Icon
                                FROM dbo.ServiceOrders o
                                LEFT JOIN dbo.Customers c ON o.CustomerId = c.CustomerId

                                UNION ALL

                                SELECT N'Thanh toán hóa đơn #' + i.InvoiceCode AS Title,
                                       i.PaidAt AS ActivityTime,
                                       N'Thanh toán' AS Icon
                                FROM dbo.Invoices i
                                WHERE i.PaidAt IS NOT NULL

                                UNION ALL

                                SELECT N'Thêm dịch vụ ' + s.ServiceName AS Title,
                                       s.CreatedAt AS ActivityTime,
                                       N'Thêm dịch vụ' AS Icon
                                FROM dbo.Services s
                            ) AS Acts
                            ORDER BY ActivityTime DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RecentActivityDTO
                {
                    Title = row["Title"].ToString(),
                    Timestamp = Convert.ToDateTime(row["ActivityTime"]),
                    Icon = row["Icon"].ToString()
                });
            }

            return list;
        }

        private string GetDayOfWeekLabel(DayOfWeek dow)
        {
            switch (dow)
            {
                case DayOfWeek.Monday: return "T2";
                case DayOfWeek.Tuesday: return "T3";
                case DayOfWeek.Wednesday: return "T4";
                case DayOfWeek.Thursday: return "T5";
                case DayOfWeek.Friday: return "T6";
                case DayOfWeek.Saturday: return "T7";
                case DayOfWeek.Sunday: return "CN";
                default: return "";
            }
        }
    }
}
