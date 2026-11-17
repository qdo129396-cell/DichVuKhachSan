using System;
using System.Collections.Generic;
using DichVuKhachSan.DAL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan.BLL
{
    public class UserBLL
    {
        private readonly UserDAL _dal = new UserDAL();
        public List<UserDTO> GetAll() => _dal.GetAll();
        public bool Add(UserDTO user, string password, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.FullName) || string.IsNullOrWhiteSpace(password)) { error = "Vui lòng nhập đầy đủ tài khoản, họ tên và mật khẩu."; return false; }
            if (password.Length < 6) { error = "Mật khẩu phải có ít nhất 6 ký tự."; return false; }
            if (_dal.GetByUsername(user.Username.Trim()) != null) { error = "Tên đăng nhập đã tồn tại."; return false; }
            try { user.Username = user.Username.Trim(); user.FullName = user.FullName.Trim(); user.PasswordHash = PasswordHelper.HashPassword(password); _dal.Insert(user); return true; }
            catch (Exception ex) { error = "Không thể thêm người dùng: " + ex.Message; return false; }
        }
        public bool Update(UserDTO user, string newPassword, out string error)
        {
            error = string.Empty;
            if (user.UserId <= 0 || string.IsNullOrWhiteSpace(user.FullName)) { error = "Thông tin người dùng không hợp lệ."; return false; }
            var old = _dal.GetById(user.UserId);
            if (old == null) { error = "Người dùng không còn tồn tại."; return false; }
            if (old.Role == "Admin" && old.IsActive && (user.Role != "Admin" || !user.IsActive) && _dal.CountAdmins() <= 1) { error = "Không thể khóa hoặc hạ quyền tài khoản Admin cuối cùng."; return false; }
            try { _dal.Update(user); if (!string.IsNullOrWhiteSpace(newPassword)) { if (newPassword.Length < 6) { error = "Mật khẩu mới phải có ít nhất 6 ký tự."; return false; } _dal.UpdatePassword(user.UserId, PasswordHelper.HashPassword(newPassword)); } return true; }
            catch (Exception ex) { error = "Không thể cập nhật người dùng: " + ex.Message; return false; }
        }
        public bool Delete(int id, out string error)
        {
            error = string.Empty; var user = _dal.GetById(id);
            if (user == null) { error = "Người dùng không tồn tại."; return false; }
            if (user.Role == "Admin" && user.IsActive && _dal.CountAdmins() <= 1) { error = "Không thể xóa Admin cuối cùng."; return false; }
            try { return _dal.Delete(id); } catch (Exception ex) { error = "Không thể xóa người dùng: " + ex.Message; return false; }
        }
    }
}
