using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmQuanLyDichVu : Form
    {
        private readonly ServiceBLL _serviceBLL = new ServiceBLL();
        private List<ServiceDTO> _currentList = new List<ServiceDTO>();
        private int _selectedServiceId = 0;
        private bool _isBinding = false;

        public FrmQuanLyDichVu()
        {
            InitializeComponent();
        }

        private void FrmQuanLyDichVu_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadServices();
            ResetForm();
            dgvDichVu.CellFormatting += DgvDichVu_CellFormatting;
        }

        private void LoadCategories()
        {
            cboLoaiDV.Items.Clear();
            var categories = _serviceBLL.GetCategories();
            if (categories.Count == 0)
            {
                categories.AddRange(new[] { "Sinh hoạt", "Thư giãn", "Ăn uống", "Spa", "Giặt ủi", "Đồ uống" });
            }

            foreach (var cat in categories)
            {
                cboLoaiDV.Items.Add(cat);
            }

            if (cboLoaiDV.Items.Count > 0)
                cboLoaiDV.SelectedIndex = 0;

            if (cboTrangThai.Items.Count > 0)
                cboTrangThai.SelectedIndex = 0;
        }

        private void LoadServices(string keyword = "")
        {
            try
            {
                _isBinding = true;
                dgvDichVu.Rows.Clear();

                if (string.IsNullOrWhiteSpace(keyword) || keyword == "Tìm kiếm...")
                {
                    _currentList = _serviceBLL.GetAllServices();
                }
                else
                {
                    _currentList = _serviceBLL.SearchServices(keyword);
                }

                foreach (var s in _currentList)
                {
                    string statusText = s.IsActive ? "Hoạt động" : "Ngừng KD";
                    int rowIndex = dgvDichVu.Rows.Add(s.ServiceCode, s.ServiceName, s.Category, s.UnitPrice.ToString("#,##0"), statusText);
                    dgvDichVu.Rows[rowIndex].Tag = s;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách dịch vụ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBinding = false;
            }
        }

        private void ResetForm()
        {
            _selectedServiceId = 0;
            txtMaDV.Text = _serviceBLL.GetNextServiceCode();
            txtTenDV.Text = "";
            txtGiaDV.Text = "0";
            if (cboLoaiDV.Items.Count > 0) cboLoaiDV.SelectedIndex = 0;
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
            txtTenDV.Focus();
        }

        private void dgvDichVu_SelectionChanged(object sender, EventArgs e)
        {
            if (_isBinding) return;

            if (dgvDichVu.SelectedRows.Count > 0)
            {
                var row = dgvDichVu.SelectedRows[0];
                if (row.Tag is ServiceDTO s)
                {
                    _selectedServiceId = s.ServiceId;
                    txtMaDV.Text = s.ServiceCode;
                    txtTenDV.Text = s.ServiceName;
                    txtGiaDV.Text = s.UnitPrice.ToString("#,##0");

                    int catIdx = cboLoaiDV.FindStringExact(s.Category);
                    if (catIdx >= 0) cboLoaiDV.SelectedIndex = catIdx;
                    else
                    {
                        cboLoaiDV.Items.Add(s.Category);
                        cboLoaiDV.SelectedItem = s.Category;
                    }

                    cboTrangThai.SelectedIndex = s.IsActive ? 0 : 1;
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string code = txtMaDV.Text.Trim();
            string name = txtTenDV.Text.Trim();
            string category = cboLoaiDV.SelectedItem?.ToString() ?? "Sinh hoạt";
            bool isActive = cboTrangThai.SelectedIndex == 0;

            string priceRaw = txtGiaDV.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(priceRaw, out decimal price))
            {
                MessageBox.Show("Giá dịch vụ không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaDV.Focus();
                return;
            }

            var service = new ServiceDTO
            {
                ServiceCode = code,
                ServiceName = name,
                Category = category,
                UnitPrice = price,
                IsActive = isActive,
                Unit = "Lần"
            };

            if (_serviceBLL.AddService(service, out string err))
            {
                MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadServices();
                ResetForm();
            }
            else
            {
                MessageBox.Show(err, "Không thể thêm dịch vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_selectedServiceId <= 0)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần sửa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = txtMaDV.Text.Trim();
            string name = txtTenDV.Text.Trim();
            string category = cboLoaiDV.SelectedItem?.ToString() ?? "Sinh hoạt";
            bool isActive = cboTrangThai.SelectedIndex == 0;

            string priceRaw = txtGiaDV.Text.Replace(",", "").Replace(".", "").Trim();
            if (!decimal.TryParse(priceRaw, out decimal price))
            {
                MessageBox.Show("Giá dịch vụ không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaDV.Focus();
                return;
            }

            var service = new ServiceDTO
            {
                ServiceId = _selectedServiceId,
                ServiceCode = code,
                ServiceName = name,
                Category = category,
                UnitPrice = price,
                IsActive = isActive,
                Unit = "Lần"
            };

            if (_serviceBLL.UpdateService(service, out string err))
            {
                MessageBox.Show("Cập nhật dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadServices();
                ResetForm();
            }
            else
            {
                MessageBox.Show(err, "Không thể cập nhật dịch vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_selectedServiceId <= 0)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa dịch vụ '{txtTenDV.Text}' không?",
                "Xác nhận xóa dịch vụ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                if (_serviceBLL.DeleteService(_selectedServiceId, out string msg))
                {
                    MessageBox.Show(string.IsNullOrEmpty(msg) ? "Xóa dịch vụ thành công!" : msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadServices();
                    ResetForm();
                }
                else
                {
                    MessageBox.Show(msg, "Lỗi xóa dịch vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "Tìm kiếm...";
            txtTimKiem.ForeColor = Color.Gray;
            LoadServices();
            ResetForm();
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            if (txtTimKiem.Text != "Tìm kiếm...")
            {
                LoadServices(txtTimKiem.Text.Trim());
            }
        }

        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Tìm kiếm...")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                txtTimKiem.Text = "Tìm kiếm...";
                txtTimKiem.ForeColor = Color.Gray;
                LoadServices();
            }
        }

        private void DgvDichVu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvDichVu.Columns[e.ColumnIndex].Name == "colTrangThai" && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val == "Hoạt động")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(22, 163, 74);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
                }
            }
        }

        private void picDichVu_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw stacked luxury hotel spa towels
            using (SolidBrush towel1 = new SolidBrush(Color.FromArgb(186, 230, 253)))
            using (SolidBrush towel2 = new SolidBrush(Color.FromArgb(147, 197, 253)))
            using (SolidBrush towel3 = new SolidBrush(Color.FromArgb(96, 165, 250)))
            {
                g.FillEllipse(towel3, 15, 65, 75, 22);
                g.FillEllipse(towel2, 15, 45, 75, 22);
                g.FillEllipse(towel1, 15, 25, 75, 22);
            }

            // Draw decorative flower
            using (SolidBrush flowerCenter = new SolidBrush(Color.FromArgb(254, 240, 138)))
            using (SolidBrush flowerPetal = new SolidBrush(Color.White))
            {
                g.FillEllipse(flowerPetal, 60, 15, 16, 16);
                g.FillEllipse(flowerCenter, 64, 19, 8, 8);
            }
        }
    }
}
