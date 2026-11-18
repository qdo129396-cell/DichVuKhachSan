using System;
using System.Collections.Generic;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.BLL
{
    public class ServiceOrderBLL
    {
        private readonly ServiceOrderDAL _orderDAL = new ServiceOrderDAL();

        public List<ServiceOrderDTO> GetAllOrders()
        {
            return _orderDAL.GetAllOrders();
        }

        public List<ServiceOrderDetailDTO> GetOrderDetails(int orderId)
        {
            return _orderDAL.GetDetailsByOrderId(orderId);
        }

        public string GetNextOrderCode()
        {
            return _orderDAL.GenerateNextOrderCode();
        }

        public bool CreateBooking(ServiceOrderDTO order, List<ServiceOrderDetailDTO> details, out string invoiceCode, out string errorMessage)
        {
            invoiceCode = string.Empty;
            errorMessage = string.Empty;

            if (details == null || details.Count == 0)
            {
                errorMessage = "Phiếu đặt dịch vụ phải có ít nhất một dịch vụ được chọn!";
                return false;
            }

            foreach (var item in details)
            {
                if (item.Quantity <= 0)
                {
                    errorMessage = $"Số lượng cho dịch vụ '{item.ServiceName}' phải lớn hơn 0!";
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(order.OrderCode))
            {
                order.OrderCode = _orderDAL.GenerateNextOrderCode();
            }

            if (order.CreatedBy <= 0)
            {
                order.CreatedBy = AuthService.CurrentUser != null ? AuthService.CurrentUser.UserId : 1;
            }

            try
            {
                int orderId;
                bool success = _orderDAL.CreateOrderWithDetails(order, details, out orderId, out invoiceCode);
                return success;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi lưu phiếu dịch vụ vào database: " + ex.Message;
                return false;
            }
        }

        public bool UpdateStatus(int orderId, string status, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                return _orderDAL.UpdateStatus(orderId, status);
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi cập nhật trạng thái: " + ex.Message;
                return false;
            }
        }

        public bool UpdateDetailQuantity(int orderDetailId, decimal quantity, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (quantity <= 0) { errorMessage = "Số lượng phải lớn hơn 0."; return false; }
            try { if (!_orderDAL.UpdateDetailQuantity(orderDetailId, quantity)) { errorMessage = "Không thể sửa dịch vụ đã thanh toán."; return false; } return true; }
            catch (Exception ex) { errorMessage = ex.Message; return false; }
        }

        public bool UpdateDetail(ServiceOrderDetailDTO detail, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (detail == null || detail.Quantity <= 0) { errorMessage = "Số lượng phải lớn hơn 0."; return false; }
            try { if (!_orderDAL.UpdateDetail(detail)) { errorMessage = "Không thể sửa dịch vụ đã thanh toán."; return false; } return true; }
            catch (Exception ex) { errorMessage = ex.Message; return false; }
        }
    }
}
