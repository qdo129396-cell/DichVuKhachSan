using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmQuanLyHoaDon : Form
    {
        private readonly InvoiceBLL _bll = new InvoiceBLL();
        private readonly ServiceOrderBLL _orders = new ServiceOrderBLL();
        private readonly ServiceBLL _services = new ServiceBLL();
        private readonly PrintDocument _printer = new PrintDocument();
        private ComboBox _invoicePicker;
        private List<InvoiceDTO> _invoices = new List<InvoiceDTO>(), _group = new List<InvoiceDTO>();
        private InvoiceDTO _current;
        private bool _settingCustomer;

        public FrmQuanLyHoaDon()
        {
            InitializeComponent();
            btnThanhToan.Click += Pay; btnInHoaDon.Click += Print; btnLuuHoaDon.Click += (_, __) => LoadInvoices();
            btnThemDichVu.Text = "Sửa thông tin";
            btnThemDichVu.Click += EditSelectedInfo;
            txtKhachHang.PlaceholderText = "Nhập tên khách hàng để tìm...";
            txtKhachHang.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtKhachHang.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtKhachHang.TextChanged += SearchCustomer;
            _printer.PrintPage += PrintPage;
        }

        private void FrmQuanLyHoaDon_Load(object sender, EventArgs e)
        {
            _invoicePicker = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(370, 16), Size = new Size(440, 25), Font = txtKhachHang.Font };
            _invoicePicker.SelectedIndexChanged += (_, __) => ShowGroup(_invoicePicker.SelectedItem as InvoiceDTO);
            panelTop.Controls.Add(_invoicePicker);
            cboThanhToan.Items.Clear(); cboThanhToan.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản" }); cboThanhToan.SelectedIndex = 0;
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            try
            {
                _invoices = _bll.GetAllInvoices();
                var names = new AutoCompleteStringCollection(); names.AddRange(_invoices.Select(x => x.CustomerName).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray()); txtKhachHang.AutoCompleteCustomSource = names;
                BindPicker(_invoices); if (_invoices.Count == 0) ClearView();
            }
            catch (Exception ex) { MessageBox.Show("Không tải được hóa đơn: " + ex.Message); }
        }

        private void BindPicker(List<InvoiceDTO> source)
        {
            if (_invoicePicker == null) return;
            _invoicePicker.DataSource = null; _invoicePicker.DataSource = source; _invoicePicker.DisplayMember = "InvoiceCode"; _invoicePicker.ValueMember = "InvoiceId";
        }

        private void SearchCustomer(object sender, EventArgs e)
        {
            if (_settingCustomer || _invoicePicker == null) return;
            string key = txtKhachHang.Text.Trim();
            var matches = _invoices.Where(x => key.Length == 0 || x.CustomerName.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            _settingCustomer = true; BindPicker(matches); _invoicePicker.SelectedIndex = -1; _settingCustomer = false;
            if (matches.Count == 0) ClearView();
        }

        private void ShowGroup(InvoiceDTO selected)
        {
            if (_settingCustomer) return;
            if (selected == null) return;
            _settingCustomer = true; txtKhachHang.Text = selected.CustomerName; _settingCustomer = false;
            _group = selected.CustomerId.HasValue
                ? _invoices.Where(x => x.CustomerId == selected.CustomerId && x.PaymentStatus == "Unpaid").Select(x => _bll.GetInvoiceById(x.InvoiceId)).Where(x => x != null).ToList()
                : new List<InvoiceDTO> { _bll.GetInvoiceById(selected.InvoiceId) };
            if (_group.Count == 0) _group.Add(_bll.GetInvoiceById(selected.InvoiceId));
            _current = _group[0]; dtpNgayLap.Value = _current.InvoiceDate; dgvChiTietHoaDon.Rows.Clear();
            int n = 1;
            foreach (var invoice in _group) foreach (var d in invoice.OrderDetails)
            {
                int row = dgvChiTietHoaDon.Rows.Add(n++, d.ServiceName + (_group.Count > 1 ? " (" + invoice.InvoiceCode + ")" : ""), d.Quantity.ToString("0.##"), d.UnitPrice.ToString("#,##0"), d.LineTotal.ToString("#,##0"));
                dgvChiTietHoaDon.Rows[row].Tag = d;
                dgvChiTietHoaDon.Rows[row].Cells[0].ReadOnly = dgvChiTietHoaDon.Rows[row].Cells[1].ReadOnly = dgvChiTietHoaDon.Rows[row].Cells[3].ReadOnly = dgvChiTietHoaDon.Rows[row].Cells[4].ReadOnly = true;
            }
            decimal total = _group.Sum(x => x.TotalAmount);
            lblTongTienVal.Text = total.ToString("#,##0") + " VNĐ" + (_group.Count > 1 ? " (gộp " + _group.Count + " hóa đơn)" : "");
            bool unpaid = _group.All(x => x.PaymentStatus == "Unpaid"); cboThanhToan.Enabled = btnThanhToan.Enabled = unpaid; btnThemDichVu.Enabled = unpaid && dgvChiTietHoaDon.Rows.Count > 0; btnThanhToan.Text = unpaid ? "✓ Thanh toán gộp" : "Đã thanh toán";
        }

        private void EditSelectedInfo(object sender, EventArgs e)
        {
            if (!(dgvChiTietHoaDon.CurrentRow?.Tag is ServiceOrderDetailDTO d)) { MessageBox.Show("Vui lòng chọn dịch vụ cần sửa."); return; }
            var serviceList = _services.GetActiveServices(); if (serviceList.Count == 0) { MessageBox.Show("Chưa có dịch vụ khả dụng."); return; }
            using var dialog = new Form { Text = "Sửa thông tin dịch vụ", StartPosition = FormStartPosition.CenterParent, Size = new Size(430, 270), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
            var cbo = new ComboBox { Left = 130, Top = 25, Width = 245, DropDownStyle = ComboBoxStyle.DropDownList, DataSource = serviceList, DisplayMember = "ServiceName", ValueMember = "ServiceId" };
            cbo.SelectedValue = d.ServiceId;
            var quantity = new TextBox { Left = 130, Top = 70, Width = 245, Text = d.Quantity.ToString("0.##") };
            var notes = new TextBox { Left = 130, Top = 115, Width = 245, Height = 45, Multiline = true, Text = d.Notes ?? "" };
            dialog.Controls.AddRange(new Control[] { new Label { Text = "Dịch vụ:", Left = 25, Top = 30, AutoSize = true }, cbo, new Label { Text = "Số lượng:", Left = 25, Top = 75, AutoSize = true }, quantity, new Label { Text = "Ghi chú:", Left = 25, Top = 120, AutoSize = true }, notes });
            var save = new Button { Text = "Lưu", DialogResult = DialogResult.OK, Left = 210, Top = 185, Width = 80 }; var cancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Left = 295, Top = 185, Width = 80 }; dialog.Controls.AddRange(new Control[] { save, cancel }); dialog.AcceptButton = save; dialog.CancelButton = cancel;
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            if (!(cbo.SelectedItem is ServiceDTO selected) || !decimal.TryParse(quantity.Text, out decimal qty) || qty <= 0) { MessageBox.Show("Dịch vụ hoặc số lượng không hợp lệ."); return; }
            d.ServiceId = selected.ServiceId; d.ServiceName = selected.ServiceName; d.UnitPrice = selected.UnitPrice; d.Quantity = qty; d.Notes = notes.Text.Trim();
            if (!_orders.UpdateDetail(d, out string error)) { MessageBox.Show(error); return; }
            int id = _current.InvoiceId; LoadInvoices(); var refreshed = _invoices.FirstOrDefault(x => x.InvoiceId == id); if (refreshed != null) ShowGroup(refreshed);
        }

        private void Pay(object sender, EventArgs e)
        {
            if (_group.Count == 0 || _group.Any(x => x.PaymentStatus != "Unpaid")) { MessageBox.Show("Hóa đơn này không thể thanh toán."); return; }
            decimal total = _group.Sum(x => x.TotalAmount);
            string input = Microsoft.VisualBasic.Interaction.InputBox("Nhập tổng giảm giá cho " + _group.Count + " hóa đơn (0 nếu không giảm):", "Thanh toán gộp", "0");
            if (!decimal.TryParse(input, out decimal discount) || discount < 0 || discount > total) { MessageBox.Show("Giảm giá không hợp lệ."); return; }
            if (!_bll.PayInvoices(_group.Select(x => x.InvoiceId), discount, cboThanhToan.Text, _current.Notes, out string error)) { MessageBox.Show(error); return; }
            MessageBox.Show("Đã thanh toán gộp " + _group.Count + " hóa đơn của " + _current.CustomerName + "."); LoadInvoices();
        }

        private void ClearView() { dgvChiTietHoaDon.Rows.Clear(); _current = null; _group.Clear(); lblTongTienVal.Text = "0 VNĐ"; btnThanhToan.Enabled = btnThemDichVu.Enabled = false; }
        private void Print(object sender, EventArgs e) { if (_current == null) { MessageBox.Show("Vui lòng chọn khách hàng/hóa đơn."); return; } using var dialog = new PrintDialog { Document = _printer, UseEXDialog = true }; if (dialog.ShowDialog() == DialogResult.OK) _printer.Print(); }
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_current == null) return; int y = e.MarginBounds.Top; using var title = new Font("Segoe UI", 16, FontStyle.Bold); using var body = new Font("Segoe UI", 10);
            e.Graphics.DrawString("HOTEL SERVICE - HÓA ĐƠN", title, Brushes.Navy, e.MarginBounds.Left, y); y += 45; e.Graphics.DrawString("Khách hàng: " + _current.CustomerName, body, Brushes.Black, e.MarginBounds.Left, y); y += 35;
            foreach (var invoice in _group) foreach (var d in invoice.OrderDetails) { e.Graphics.DrawString(d.ServiceName + " x" + d.Quantity.ToString("0.##") + "    " + d.LineTotal.ToString("#,##0") + " VNĐ", body, Brushes.Black, e.MarginBounds.Left, y); y += 22; }
            y += 12; e.Graphics.DrawString("TỔNG THANH TOÁN: " + _group.Sum(x => x.FinalAmount).ToString("#,##0") + " VNĐ", title, Brushes.Black, e.MarginBounds.Left, y);
        }
    }
}
