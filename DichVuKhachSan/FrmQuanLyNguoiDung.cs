using System;
using System.Drawing;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmQuanLyNguoiDung : Form
    {
        private readonly UserBLL _bll = new UserBLL(); private int _selectedId;
        public FrmQuanLyNguoiDung() { InitializeComponent(); btnThem.Click += (_, __) => Save(false); btnSua.Click += (_, __) => Save(true); btnXoa.Click += (_, __) => Delete(); btnLamMoi.Click += (_, __) => Reset(); dgvNguoiDung.SelectionChanged += (_, __) => SelectRow(); }
        private void FrmQuanLyNguoiDung_Load(object sender, EventArgs e) { cboVaiTro.Items.Clear(); cboVaiTro.Items.AddRange(new object[] { "Staff", "Manager", "Admin" }); cboTrangThai.SelectedIndex = 0; txtMatKhau.Clear(); LoadData(); Reset(); }
        private void LoadData() { try { dgvNguoiDung.Rows.Clear(); foreach (var x in _bll.GetAll()) { var i = dgvNguoiDung.Rows.Add(x.Username, x.FullName, x.Role, x.IsActive ? "Hoạt động" : "Khóa"); dgvNguoiDung.Rows[i].Tag = x; } } catch (Exception ex) { MessageBox.Show("Không tải được người dùng: " + ex.Message); } }
        private void SelectRow() { if (dgvNguoiDung.SelectedRows.Count == 0 || !(dgvNguoiDung.SelectedRows[0].Tag is UserDTO x)) return; _selectedId = x.UserId; txtTaiKhoan.Text = x.Username; txtTaiKhoan.ReadOnly = true; txtHoTen.Text = x.FullName; txtMatKhau.Clear(); cboVaiTro.SelectedItem = x.Role; cboTrangThai.SelectedIndex = x.IsActive ? 0 : 1; }
        private void Save(bool update) { if (update && _selectedId <= 0) { MessageBox.Show("Vui lòng chọn tài khoản cần sửa."); return; } var u = new UserDTO { UserId = _selectedId, Username = txtTaiKhoan.Text.Trim(), FullName = txtHoTen.Text.Trim(), Role = cboVaiTro.SelectedItem?.ToString() ?? "Staff", IsActive = cboTrangThai.SelectedIndex == 0 }; var ok = update ? _bll.Update(u, txtMatKhau.Text, out var err) : _bll.Add(u, txtMatKhau.Text, out err); if (!ok) { MessageBox.Show(err); return; } MessageBox.Show(update ? "Cập nhật tài khoản thành công." : "Thêm tài khoản thành công."); LoadData(); Reset(); }
        private void Delete() { if (_selectedId <= 0) { MessageBox.Show("Vui lòng chọn tài khoản cần xóa."); return; } if (MessageBox.Show("Xóa tài khoản đang chọn?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return; if (!_bll.Delete(_selectedId, out var e)) MessageBox.Show(e); else { LoadData(); Reset(); } }
        private void Reset() { _selectedId = 0; txtTaiKhoan.ReadOnly = false; txtTaiKhoan.Clear(); txtHoTen.Clear(); txtMatKhau.Clear(); cboVaiTro.SelectedItem = "Staff"; cboTrangThai.SelectedIndex = 0; dgvNguoiDung.ClearSelection(); }
    }
}
