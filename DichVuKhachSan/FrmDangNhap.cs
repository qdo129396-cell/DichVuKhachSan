using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DAL;

namespace DichVuKhachSan
{
    public partial class FrmDangNhap : Form
    {
        public FrmDangNhap()
        {
            InitializeComponent();
        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {
            // 1. Kiểm tra kết nối Database khi khởi động
            if (!DatabaseHelper.TestConnection(out string dbError))
            {
                DatabaseHelper.IsOfflineMode = true;
                txtUsername.Text = "offline";
            }

            // 2. Kiểm tra nếu chưa có tài khoản nào trong hệ thống, đề xuất tạo tài khoản Admin đầu tiên
            if (!AuthService.HasAnyUser())
            {
                var ask = MessageBox.Show(
                    "Hệ thống chưa có tài khoản quản trị nào trong cơ sở dữ liệu.\n\n" +
                    "Bạn có muốn tạo tài khoản Admin mặc định:\n" +
                    "• Tên đăng nhập: admin\n" +
                    "• Mật khẩu: 123456\n" +
                    "• Vai trò: Admin\n\n" +
                    "để bắt đầu sử dụng ngay không?",
                    "Thiết lập tài khoản Admin đầu tiên",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (ask == DialogResult.Yes)
                {
                    if (AuthService.CreateInitialAdmin("admin", "123456", "Quản trị viên hệ thống", out string err))
                    {
                        txtUsername.Text = "admin";
                        txtPassword.Text = "123456";
                        MessageBox.Show("Khởi tạo tài khoản Admin thành công! Mật khẩu đã được mã hóa an toàn bằng PBKDF2.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(err, "Lỗi tạo tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void panelLeft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = panelLeft.Width;
            int h = panelLeft.Height;

            // 1. Sky & Resort gradient background
            using (LinearGradientBrush skyBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, h),
                Color.FromArgb(30, 58, 138),
                Color.FromArgb(15, 23, 42)))
            {
                g.FillRectangle(skyBrush, 0, 0, w, h);
            }

            // 2. Hotel Resort Facade Architecture (Modern Luxury Hotel silhouette)
            using (SolidBrush hotelMainBrush = new SolidBrush(Color.FromArgb(70, 241, 245, 249)))
            using (SolidBrush windowLitBrush = new SolidBrush(Color.FromArgb(220, 253, 224, 71))) // Warm golden window glow
            using (SolidBrush windowDimBrush = new SolidBrush(Color.FromArgb(100, 254, 240, 138)))
            {
                // Main building center block
                g.FillRectangle(hotelMainBrush, 90, 40, 210, 200);

                // Left wing
                g.FillRectangle(hotelMainBrush, 30, 80, 70, 160);

                // Right wing
                g.FillRectangle(hotelMainBrush, 290, 80, 70, 160);

                // Roof pediment / classical top arch
                Point[] roofPoints = { new Point(80, 40), new Point(195, 10), new Point(310, 40) };
                g.FillPolygon(hotelMainBrush, roofPoints);

                // Windows grid with warm lighting
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 6; col++)
                    {
                        int wx = 105 + (col * 30);
                        int wy = 55 + (row * 32);
                        Brush b = (row + col) % 3 == 0 ? windowDimBrush : windowLitBrush;
                        g.FillRectangle(b, wx, wy, 18, 20);
                    }
                }
            }

            // 3. Palm tree silhouette (resort tropical aesthetic)
            using (SolidBrush palmBrush = new SolidBrush(Color.FromArgb(140, 30, 41, 59)))
            {
                // Left palm trunk & fronds
                g.FillPolygon(palmBrush, new Point[] { new Point(25, 230), new Point(32, 130), new Point(36, 230) });
                g.FillEllipse(palmBrush, 5, 115, 55, 25);

                // Right palm trunk & fronds
                g.FillPolygon(palmBrush, new Point[] { new Point(355, 230), new Point(362, 130), new Point(366, 230) });
                g.FillEllipse(palmBrush, 335, 115, 55, 25);
            }

            // 4. Modern Curved Ocean Blue Wave overlay at the bottom
            using (GraphicsPath wavePath = new GraphicsPath())
            {
                wavePath.AddBezier(0, h - 210, w * 0.35f, h - 260, w * 0.65f, h - 170, w, h - 220);
                wavePath.AddLine(w, h, 0, h);
                wavePath.CloseFigure();

                using (LinearGradientBrush waveBrush = new LinearGradientBrush(
                    new Point(0, h - 260),
                    new Point(0, h),
                    Color.FromArgb(235, 13, 71, 161),
                    Color.FromArgb(250, 10, 37, 64)))
                {
                    g.FillPath(waveBrush, wavePath);
                }
            }

            // Subtle highlight line on the wave crest
            using (Pen crestPen = new Pen(Color.FromArgb(90, 56, 189, 248), 2.5f))
            {
                g.DrawBezier(crestPen, 0, h - 210, w * 0.35f, h - 260, w * 0.65f, h - 170, w, h - 220);
            }
        }

        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnTogglePassword.Text = txtPassword.UseSystemPasswordChar ? "👁" : "🙈";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (DatabaseHelper.IsOfflineMode)
            {
                AuthService.BeginOfflineSession(username);
                FrmTrangChu offlineMainForm = new FrmTrangChu();
                this.Hide();
                offlineMainForm.FormClosed += (s, args) => this.Close();
                offlineMainForm.Show();
                return;
            }

            // Xử lý tạo admin nếu database chưa có người dùng
            if (!AuthService.HasAnyUser())
            {
                if (AuthService.CreateInitialAdmin(username, password, "Quản trị viên", out string errCreate))
                {
                    MessageBox.Show("Đã tạo tài khoản Quản trị viên đầu tiên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(errCreate, "Lỗi tạo tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (AuthService.Login(username, password, out string errorMessage))
            {
                FrmTrangChu mainForm = new FrmTrangChu();
                this.Hide();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show(errorMessage, "Đăng nhập không thành công", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
