using System;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmQuanLyKhachHang : Form
    {
        private readonly CustomerBLL _bll = new CustomerBLL(); private int _selectedId;
        public FrmQuanLyKhachHang() { InitializeComponent(); btnThem.Click += (_, __) => Save(false); btnSua.Click += (_, __) => Save(true); btnXoa.Click += (_, __) => Delete(); btnLamMoi.Click += (_, __) => ResetForm(); txtTimKiem.TextChanged += (_, __) => LoadData(txtTimKiem.Text == "Tìm kiếm..." ? "" : txtTimKiem.Text); dgvKhachHang.SelectionChanged += (_, __) => SelectRow(); }
        private void FrmQuanLyKhachHang_Load(object sender, EventArgs e) { LoadData(); ResetForm(); }
        private void LoadData(string key = "") { try { dgvKhachHang.Rows.Clear(); foreach (var x in _bll.SearchCustomers(key)) { var i = dgvKhachHang.Rows.Add(x.CustomerCode, x.FullName, x.Phone, x.Email); dgvKhachHang.Rows[i].Tag = x; } } catch (Exception ex) { MessageBox.Show("Không tải được khách hàng: " + ex.Message); } }
        private void SelectRow() { if (dgvKhachHang.SelectedRows.Count == 0 || !(dgvKhachHang.SelectedRows[0].Tag is CustomerDTO x)) return; _selectedId = x.CustomerId; txtMaKH.Text = x.CustomerCode; txtHoTen.Text = x.FullName; txtSDT.Text = x.Phone; txtEmail.Text = x.Email; txtDiaChi.Text = x.Address; }
        private CustomerDTO Read() => new CustomerDTO { CustomerId = _selectedId, CustomerCode = txtMaKH.Text.Trim(), FullName = txtHoTen.Text.Trim(), Phone = txtSDT.Text.Trim(), Email = txtEmail.Text.Trim(), Address = txtDiaChi.Text.Trim(), IsActive = true };
        private void Save(bool update) { if (update && _selectedId <= 0) { MessageBox.Show("Vui lòng chọn khách hàng cần sửa."); return; } var ok = update ? _bll.UpdateCustomer(Read(), out var err) : _bll.AddCustomer(Read(), out err); if (!ok) { MessageBox.Show(err, "Dữ liệu không hợp lệ"); return; } MessageBox.Show(update ? "Cập nhật khách hàng thành công." : "Thêm khách hàng thành công."); LoadData(); ResetForm(); }
        private void Delete() { if (_selectedId <= 0) { MessageBox.Show("Vui lòng chọn khách hàng cần xóa."); return; } if (MessageBox.Show("Xóa khách hàng đang chọn?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return; if (!_bll.DeleteCustomer(_selectedId, out var err)) { MessageBox.Show(err); return; } LoadData(); ResetForm(); }
        private void ResetForm() { _selectedId = 0; txtMaKH.Text = _bll.GetNextCustomerCode(); txtHoTen.Clear(); txtSDT.Clear(); txtEmail.Clear(); txtDiaChi.Clear(); dgvKhachHang.ClearSelection(); }
    }
}
