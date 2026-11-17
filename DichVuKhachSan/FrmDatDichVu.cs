using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmDatDichVu : Form
    {
        private readonly CustomerBLL _customers = new CustomerBLL(); private readonly ServiceBLL _services = new ServiceBLL(); private readonly ServiceOrderBLL _orders = new ServiceOrderBLL();
        public FrmDatDichVu() { InitializeComponent(); btnDatDichVu.Click += Book; cboDichVu.SelectedIndexChanged += UpdatePreview; txtSoLuong.TextChanged += UpdatePreview; }
        private void FrmDatDichVu_Load(object sender, EventArgs e) { try { cboKhachHang.DataSource = _customers.GetAllCustomers(); cboKhachHang.DisplayMember = "FullName"; cboKhachHang.ValueMember = "CustomerId"; cboDichVu.DataSource = _services.GetActiveServices(); cboDichVu.DisplayMember = "ServiceName"; cboDichVu.ValueMember = "ServiceId"; txtSoLuong.Text = "1"; txtGhiChu.Clear(); UpdatePreview(null, null); } catch (Exception ex) { MessageBox.Show("Không tải được dữ liệu đặt dịch vụ: " + ex.Message); } }
        private void UpdatePreview(object sender, EventArgs e) { if (!(cboDichVu.SelectedItem is ServiceDTO s)) return; if (!decimal.TryParse(txtSoLuong.Text, out var q) || q <= 0) q = 1; lblPreviewDichVuVal.Text = s.ServiceName; lblPreviewDonGiaVal.Text = s.UnitPrice.ToString("#,##0") + " VNĐ"; lblPreviewSoLuongVal.Text = q.ToString("0.##"); lblThanhTienVal.Text = (s.UnitPrice * q).ToString("#,##0") + " VNĐ"; }
        private void Book(object sender, EventArgs e) { if (!(cboDichVu.SelectedItem is ServiceDTO s)) { MessageBox.Show("Chưa có dịch vụ để đặt."); return; } if (!decimal.TryParse(txtSoLuong.Text, out var q) || q <= 0) { MessageBox.Show("Số lượng phải lớn hơn 0."); return; } var c = cboKhachHang.SelectedItem as CustomerDTO; var note = txtGhiChu.Text.Trim(); var order = new ServiceOrderDTO { CustomerId = c?.CustomerId, CreatedBy = AuthService.CurrentUser?.UserId ?? 0, OrderDate = dtpNgaySuDung.Value, Status = "Pending", Notes = note }; var details = new List<ServiceOrderDetailDTO> { new ServiceOrderDetailDTO { ServiceId = s.ServiceId, ServiceName = s.ServiceName, Quantity = q, UnitPrice = s.UnitPrice, Notes = note } }; if (!_orders.CreateBooking(order, details, out var invoice, out var err)) { MessageBox.Show(err); return; } MessageBox.Show("Đặt dịch vụ thành công. Hóa đơn được tạo: " + invoice); txtSoLuong.Text = "1"; txtGhiChu.Clear(); }
    }
}
