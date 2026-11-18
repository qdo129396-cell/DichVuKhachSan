using System;
using System.Collections.Generic;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;
using System.Net.Mail;

namespace DichVuKhachSan.BLL
{
    public class CustomerBLL
    {
        private readonly CustomerDAL _customerDAL = new CustomerDAL();

        public List<CustomerDTO> GetAllCustomers()
        {
            return _customerDAL.GetAll();
        }

        public List<CustomerDTO> SearchCustomers(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return _customerDAL.GetAll();

            return _customerDAL.Search(keyword);
        }

        public string GetNextCustomerCode()
        {
            return _customerDAL.GenerateNextCustomerCode();
        }

        public bool AddCustomer(CustomerDTO customer, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(customer.FullName))
            {
                errorMessage = "Họ tên khách hàng không được để trống!";
                return false;
            }
            if (!IsValid(customer, out errorMessage)) return false;

            if (string.IsNullOrWhiteSpace(customer.CustomerCode))
            {
                customer.CustomerCode = _customerDAL.GenerateNextCustomerCode();
            }

            // Check if code exists
            if (_customerDAL.GetByCode(customer.CustomerCode) != null)
            {
                customer.CustomerCode = _customerDAL.GenerateNextCustomerCode();
            }

            try
            {
                _customerDAL.Insert(customer);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi lưu khách hàng: " + ex.Message;
                return false;
            }
        }

        public bool UpdateCustomer(CustomerDTO customer, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(customer.FullName))
            {
                errorMessage = "Họ tên khách hàng không được để trống!";
                return false;
            }
            if (!IsValid(customer, out errorMessage)) return false;

            try
            {
                return _customerDAL.Update(customer);
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi cập nhật khách hàng: " + ex.Message;
                return false;
            }
        }

        private static bool IsValid(CustomerDTO customer, out string error)
        {
            error = string.Empty;
            if (!string.IsNullOrWhiteSpace(customer.Phone) && (customer.Phone.Length < 9 || customer.Phone.Length > 20)) { error = "Số điện thoại không hợp lệ."; return false; }
            if (!string.IsNullOrWhiteSpace(customer.Email)) try { _ = new MailAddress(customer.Email); } catch { error = "Email không hợp lệ."; return false; }
            return true;
        }

        public bool DeleteCustomer(int customerId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_customerDAL.HasOrders(customerId))
            {
                errorMessage = "Khách hàng này đã phát sinh phiếu đặt dịch vụ hoặc hóa đơn, không thể xóa để đảm bảo toàn vẹn dữ liệu!";
                return false;
            }

            try
            {
                return _customerDAL.Delete(customerId);
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi xóa khách hàng: " + ex.Message;
                return false;
            }
        }
    }
}
