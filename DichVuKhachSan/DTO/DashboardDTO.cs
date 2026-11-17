using System;
using System.Collections.Generic;

namespace DichVuKhachSan.DTO
{
    public class DashboardStatsDTO
    {
        public int TotalCustomers { get; set; }
        public int TotalServices { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TodayRevenue { get; set; }

        public string CustomerGrowth { get; set; } = "+12%";
        public string ServiceGrowth { get; set; } = "+0%";
        public string InvoiceGrowth { get; set; } = "+8%";
        public string RevenueGrowth { get; set; } = "+15%";

        public List<DailyRevenueDTO> Revenue7Days { get; set; } = new List<DailyRevenueDTO>();
        public List<RecentActivityDTO> RecentActivities { get; set; } = new List<RecentActivityDTO>();
    }

    public class DailyRevenueDTO
    {
        public DateTime Date { get; set; }
        public string DayLabel { get; set; } // T2, T3...
        public int InvoiceCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CategoryRevenueDTO
    {
        public string Category { get; set; }
        public decimal Revenue { get; set; }
        public float Percentage { get; set; }
    }

    public class RecentActivityDTO
    {
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }
        public string FormattedTime => Timestamp.ToString("HH:mm - dd/MM/yyyy");
        public string Icon { get; set; }
    }
}
