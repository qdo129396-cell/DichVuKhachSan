using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmTrangChu : Form
    {
        private Form activeChildForm = null;
        private Button currentSelectedButton = null;
        private readonly ReportBLL _reportBLL = new ReportBLL();
        private DashboardStatsDTO _currentStats = null;

        public FrmTrangChu()
        {
            InitializeComponent();
            currentSelectedButton = btnMenuTrangChu;
        }

        private void FrmTrangChu_Load(object sender, EventArgs e)
        {
            ApplyUserPermissions();
            LoadDashboardData();
        }

        private void ApplyUserPermissions()
        {
            if (AuthService.CurrentUser != null)
            {
                lblGreeting.Text = $"👋 Chào mừng, {AuthService.CurrentUser.FullName}!";
                lblSubGreeting.Text = $"Vai trò: {AuthService.CurrentUser.Role} - Chúc bạn một ngày làm việc hiệu quả!";
                lblUserProfile.Text = $"👤  {AuthService.CurrentUser.Username} ({AuthService.CurrentUser.Role})  ▼";

                // Phân quyền menu
                if (!AuthService.CanManageUsers())
                {
                    btnMenuNguoiDung.Visible = false; // Staff & Manager không được truy cập quản lý người dùng
                }

                if (!AuthService.CanViewReports())
                {
                    btnMenuThongKe.Visible = false; // Staff không xem thống kê
                }
            }
            else
            {
                lblGreeting.Text = "👋 Chào mừng, Quản trị viên!";
                lblUserProfile.Text = "👤  Admin  ▼";
            }
        }

        public void LoadDashboardData()
        {
            try
            {
                _currentStats = _reportBLL.GetDashboardStats();

                // 4 Thẻ KPI
                lblKpiCustVal.Text = _currentStats.TotalCustomers.ToString("#,##0");
                lblKpiSerVal.Text = _currentStats.TotalServices.ToString("#,##0");
                lblKpiInvVal.Text = _currentStats.TotalInvoices.ToString("#,##0");
                lblKpiRevVal.Text = _currentStats.TodayRevenue.ToString("#,##0") + " VNĐ";

                // Hoạt động gần đây
                UpdateRecentActivities(_currentStats.RecentActivities);

                // Vẽ lại biểu đồ
                panelChartCanvas.Invalidate();
            }
            catch (Exception ex)
            {
                // Fallback nếu có lỗi
                System.Diagnostics.Debug.WriteLine("Lỗi nạp dữ liệu dashboard: " + ex.Message);
            }
        }

        private void UpdateRecentActivities(List<RecentActivityDTO> activities)
        {
            if (activities == null) return;

            Panel[] actPanels = { panelAct1, panelAct2, panelAct3, panelAct4 };
            Label[] titleLabels = { lblAct1Title, lblAct2Title, lblAct3Title, lblAct4Title };
            Label[] timeLabels = { lblAct1Time, lblAct2Time, lblAct3Time, lblAct4Time };

            for (int i = 0; i < actPanels.Length; i++)
            {
                if (i < activities.Count)
                {
                    actPanels[i].Visible = true;
                    titleLabels[i].Text = activities[i].Title;
                    timeLabels[i].Text = activities[i].FormattedTime;
                }
                else
                {
                    actPanels[i].Visible = false;
                }
            }
        }

        private void HighlightButton(Button btn)
        {
            if (currentSelectedButton != null)
            {
                currentSelectedButton.BackColor = Color.FromArgb(13, 71, 161);
                currentSelectedButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            }

            currentSelectedButton = btn;
            currentSelectedButton.BackColor = Color.FromArgb(30, 136, 229);
            currentSelectedButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }

        private void ShowDashboard(Button btn)
        {
            if (activeChildForm != null)
            {
                activeChildForm.Close();
                activeChildForm = null;
            }

            HighlightButton(btn);
            panelChildContainer.Visible = false;
            panelDashboard.Visible = true;
            panelDashboard.BringToFront();

            // Refresh data khi quay lại dashboard
            LoadDashboardData();
        }

        private void OpenChildForm(Form childForm, Button btn)
        {
            if (activeChildForm != null)
            {
                activeChildForm.Close();
            }

            HighlightButton(btn);

            activeChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildContainer.Controls.Clear();
            panelChildContainer.Controls.Add(childForm);
            panelChildContainer.Tag = childForm;

            panelDashboard.Visible = false;
            panelChildContainer.Visible = true;
            panelChildContainer.BringToFront();

            childForm.BringToFront();
            childForm.Show();
        }

        private void btnMenuTrangChu_Click(object sender, EventArgs e)
        {
            ShowDashboard(btnMenuTrangChu);
        }

        private void btnMenuDichVu_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmQuanLyDichVu(), btnMenuDichVu);
        }

        private void btnMenuKhachHang_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmQuanLyKhachHang(), btnMenuKhachHang);
        }

        private void btnMenuDatDichVu_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmDatDichVu(), btnMenuDatDichVu);
        }

        private void btnMenuHoaDon_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmQuanLyHoaDon(), btnMenuHoaDon);
        }

        private void btnMenuThongKe_Click(object sender, EventArgs e)
        {
            if (!AuthService.CanViewReports())
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng Thống kê doanh thu!", "Thông báo phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenChildForm(new FrmThongKeDoanhThu(), btnMenuThongKe);
        }

        private void btnMenuNguoiDung_Click(object sender, EventArgs e)
        {
            if (!AuthService.CanManageUsers())
            {
                MessageBox.Show("Chỉ tài khoản Quản trị viên (Admin) mới có quyền quản lý người dùng!", "Thông báo phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            OpenChildForm(new FrmQuanLyNguoiDung(), btnMenuNguoiDung);
        }

        private void btnMenuDangXuat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?", "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AuthService.Logout();
                FrmDangNhap loginForm = new FrmDangNhap();
                this.Hide();
                loginForm.Show();
            }
        }

        private void panelChartCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int leftPadding = 55;
            int rightPadding = 30;
            int topPadding = 20;
            int bottomPadding = 40;

            int w = panelChartCanvas.Width - leftPadding - rightPadding;
            int h = panelChartCanvas.Height - topPadding - bottomPadding;

            if (w <= 0 || h <= 0) return;

            // Lấy dữ liệu 7 ngày từ _currentStats hoặc fallback
            var rev7Days = _currentStats?.Revenue7Days;
            if (rev7Days == null || rev7Days.Count == 0)
            {
                rev7Days = new List<DailyRevenueDTO>();
                for (int i = 6; i >= 0; i--)
                {
                    DateTime d = DateTime.Today.AddDays(-i);
                    rev7Days.Add(new DailyRevenueDTO { Date = d, DayLabel = d.ToString("ddd"), Revenue = 0 });
                }
            }

            // Tìm max value để normalize
            decimal maxVal = 15000000m; // Default 15M
            foreach (var item in rev7Days)
            {
                if (item.Revenue > maxVal) maxVal = item.Revenue * 1.2m;
            }
            if (maxVal <= 0) maxVal = 10000000m;

            // Draw Y-Axis Gridlines & Labels (0M, 1/3, 2/3, Max)
            string[] yLabels = {
                (maxVal / 1000000m).ToString("0.#") + "M",
                (maxVal * 2m / 3m / 1000000m).ToString("0.#") + "M",
                (maxVal / 3m / 1000000m).ToString("0.#") + "M",
                "0M"
            };

            using (Pen gridPen = new Pen(Color.FromArgb(226, 232, 240), 1))
            using (Font font = new Font("Segoe UI", 8.5F, FontStyle.Regular))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                gridPen.DashStyle = DashStyle.Dash;
                for (int i = 0; i < 4; i++)
                {
                    int y = topPadding + (i * h / 3);
                    g.DrawLine(gridPen, leftPadding, y, leftPadding + w, y);
                    g.DrawString(yLabels[i], font, textBrush, 8, y - 8);
                }
            }

            // X-Axis points
            PointF[] points = new PointF[rev7Days.Count];
            int stepX = rev7Days.Count > 1 ? w / (rev7Days.Count - 1) : w;

            using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                for (int i = 0; i < rev7Days.Count; i++)
                {
                    int x = leftPadding + (i * stepX);
                    float normalizedVal = (float)(rev7Days[i].Revenue / maxVal);
                    if (normalizedVal > 1f) normalizedVal = 1f;
                    int y = topPadding + (int)((1.0f - normalizedVal) * h);
                    points[i] = new PointF(x, y);

                    // Draw X Label
                    string lbl = rev7Days[i].DayLabel;
                    SizeF labelSize = g.MeasureString(lbl, font);
                    g.DrawString(lbl, font, textBrush, x - (labelSize.Width / 2), topPadding + h + 12);
                }
            }

            // Draw Area Gradient under the line
            using (GraphicsPath areaPath = new GraphicsPath())
            {
                areaPath.AddLines(points);
                areaPath.AddLine(points[points.Length - 1].X, topPadding + h, points[0].X, topPadding + h);
                areaPath.CloseFigure();

                using (LinearGradientBrush fillBrush = new LinearGradientBrush(
                    new Point(0, topPadding),
                    new Point(0, topPadding + h),
                    Color.FromArgb(70, 13, 110, 253),
                    Color.FromArgb(5, 13, 110, 253)))
                {
                    g.FillPath(fillBrush, areaPath);
                }
            }

            // Draw Blue Line
            using (Pen linePen = new Pen(Color.FromArgb(13, 110, 253), 2.5f))
            {
                g.DrawLines(linePen, points);
            }

            // Draw Dots on points
            using (Brush outerBrush = new SolidBrush(Color.FromArgb(13, 110, 253)))
            using (Brush innerBrush = new SolidBrush(Color.White))
            {
                foreach (PointF pt in points)
                {
                    g.FillEllipse(outerBrush, pt.X - 5, pt.Y - 5, 10, 10);
                    g.FillEllipse(innerBrush, pt.X - 2.5f, pt.Y - 2.5f, 5, 5);
                }
            }
        }
    }
}
