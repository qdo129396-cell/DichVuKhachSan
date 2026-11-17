using System;
using System.Collections.Generic;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.BLL
{
    public class ReportBLL
    {
        private readonly ReportDAL _reportDAL = new ReportDAL();

        public DashboardStatsDTO GetDashboardStats()
        {
            return _reportDAL.GetDashboardStats();
        }

        public List<DailyRevenueDTO> GetDailyRevenue(DateTime fromDate, DateTime toDate)
        {
            return _reportDAL.GetDailyRevenueRange(fromDate, toDate);
        }

        public List<CategoryRevenueDTO> GetCategoryRevenue(DateTime fromDate, DateTime toDate)
        {
            return _reportDAL.GetCategoryRevenueRange(fromDate, toDate);
        }

        public void GetInvoiceCounts(DateTime? fromDate, DateTime? toDate, out int totalCount, out int paidCount, out int unpaidCount, out decimal totalRevenue)
        {
            _reportDAL.GetInvoiceCounts(fromDate, toDate, out totalCount, out paidCount, out unpaidCount, out totalRevenue);
        }
    }
}
