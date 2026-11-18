using System;
using System.Collections.Generic;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.BLL
{
    public class ServiceBLL
    {
        private readonly ServiceDAL _serviceDAL = new ServiceDAL();

        public List<ServiceDTO> GetAllServices()
        {
            return _serviceDAL.GetAll();
        }

        public List<ServiceDTO> GetActiveServices()
        {
            return _serviceDAL.GetActiveServices();
        }

        public List<ServiceDTO> SearchServices(string keyword, string category = "")
        {
            return _serviceDAL.Search(keyword, category);
        }

        public List<string> GetCategories()
        {
            return _serviceDAL.GetCategories();
        }

        public string GetNextServiceCode()
        {
            return _serviceDAL.GenerateNextServiceCode();
        }

        public bool AddService(ServiceDTO service, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(service.ServiceName))
            {
                errorMessage = "Tên dịch vụ không được để trống!";
                return false;
            }

            if (service.UnitPrice < 0)
            {
                errorMessage = "Đơn giá dịch vụ không được là số âm!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(service.ServiceCode))
            {
                service.ServiceCode = _serviceDAL.GenerateNextServiceCode();
            }

            if (_serviceDAL.IsCodeExists(service.ServiceCode))
            {
                errorMessage = $"Mã dịch vụ '{service.ServiceCode}' đã tồn tại, vui lòng chọn mã khác!";
                return false;
            }

            try
            {
                _serviceDAL.Insert(service);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi thêm dịch vụ: " + ex.Message;
                return false;
            }
        }

        public bool UpdateService(ServiceDTO service, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(service.ServiceName))
            {
                errorMessage = "Tên dịch vụ không được để trống!";
                return false;
            }

            if (service.UnitPrice < 0)
            {
                errorMessage = "Đơn giá dịch vụ không được là số âm!";
                return false;
            }

            if (_serviceDAL.IsCodeExists(service.ServiceCode, service.ServiceId))
            {
                errorMessage = $"Mã dịch vụ '{service.ServiceCode}' đã bị trùng với dịch vụ khác!";
                return false;
            }

            try
            {
                return _serviceDAL.Update(service);
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi cập nhật dịch vụ: " + ex.Message;
                return false;
            }
        }

        public bool DeleteService(int serviceId, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_serviceDAL.HasOrders(serviceId))
            {
                // Soft delete by deactivating
                _serviceDAL.SetStatus(serviceId, false);
                errorMessage = "Dịch vụ đã có đơn hàng sử dụng nên hệ thống đã tự động chuyển sang trạng thái 'Ngừng kinh doanh' thay vì xóa hoàn toàn.";
                return true;
            }

            try
            {
                return _serviceDAL.Delete(serviceId);
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi xóa dịch vụ: " + ex.Message;
                return false;
            }
        }
    }
}
