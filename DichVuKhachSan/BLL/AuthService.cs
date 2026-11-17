using System;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.BLL
{
    public static class AuthService
    {
        private static readonly UserDAL _userDAL = new UserDAL();

        public static UserDTO CurrentUser { get; private set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static bool Login(string username, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Vui lòng nhập tên đăng nhập!";
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "Vui lòng nhập mật khẩu!";
                return false;
            }

            try
            {
                var user = _userDAL.GetByUsername(username.Trim());
                if (user == null)
                {
                    errorMessage = "Tài khoản không tồn tại trên hệ thống!";
                    return false;
                }

                if (!user.IsActive)
                {
                    errorMessage = "Tài khoản này đã bị khóa. Vui lòng liên hệ quản trị viên!";
                    return false;
                }

                if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    errorMessage = "Mật khẩu không chính xác!";
                    return false;
                }

                CurrentUser = user;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi kết nối cơ sở dữ liệu: " + ex.Message;
                return false;
            }
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static bool HasAnyUser()
        {
            try
            {
                return _userDAL.CountAll() > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateInitialAdmin(string username, string password, string fullName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                errorMessage = "Vui lòng điền đầy đủ thông tin tài khoản Admin!";
                return false;
            }

            try
            {
                string hash = PasswordHelper.HashPassword(password);
                var admin = new UserDTO
                {
                    Username = username.Trim(),
                    FullName = fullName.Trim(),
                    PasswordHash = hash,
                    Role = "Admin",
                    IsActive = true
                };

                _userDAL.Insert(admin);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Không thể khởi tạo tài khoản Admin: " + ex.Message;
                return false;
            }
        }

        public static bool CanManageUsers()
        {
            return CurrentUser != null && CurrentUser.Role == "Admin";
        }

        public static bool CanManageServices()
        {
            return CurrentUser != null && (CurrentUser.Role == "Admin" || CurrentUser.Role == "Manager");
        }

        public static bool CanViewReports()
        {
            return CurrentUser != null && (CurrentUser.Role == "Admin" || CurrentUser.Role == "Manager");
        }
    }
}
