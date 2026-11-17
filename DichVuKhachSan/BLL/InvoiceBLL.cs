using System;
using System.Collections.Generic;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.BLL
{
    public class InvoiceBLL
    {
        private readonly InvoiceDAL _invoiceDAL = new InvoiceDAL();

        public List<InvoiceDTO> GetAllInvoices()
        {
            return _invoiceDAL.GetAll();
        }

        public InvoiceDTO GetInvoiceById(int invoiceId)
        {
            return _invoiceDAL.GetById(invoiceId);
        }

        public List<InvoiceDTO> SearchAndFilter(string keyword, string status, DateTime? fromDate, DateTime? toDate)
        {
            return _invoiceDAL.SearchAndFilter(keyword, status, fromDate, toDate);
        }

        public bool PayInvoice(int invoiceId, decimal discountAmount, string paymentMethod, string notes, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (discountAmount < 0)
            {
                errorMessage = "Số tiền giảm giá không được là số âm!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                paymentMethod = "Tiền mặt";
            }

            try
            {
                return _invoiceDAL.ProcessPayment(invoiceId, discountAmount, paymentMethod, notes);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool CancelInvoice(int invoiceId, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                return _invoiceDAL.CancelInvoice(invoiceId);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
