using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using DichVuKhachSan.BLL;
using DichVuKhachSan.DTO;

namespace DichVuKhachSan
{
    public partial class FrmThongKeDoanhThu : Form
    {
        private readonly ReportBLL _bll = new ReportBLL();
        private List<DailyRevenueDTO> _daily = new List<DailyRevenueDTO>();
        private List<CategoryRevenueDTO> _categories = new List<CategoryRevenueDTO>();
        public FrmThongKeDoanhThu()
        {
            InitializeComponent();
            Load += (_, __) => LoadReport();
            btnThongKe.Click += (_, __) => LoadReport();
            btnTabNgay.Click += (_, __) => { dtpTuNgay.Value = DateTime.Today; dtpDenNgay.Value = DateTime.Today; LoadReport(); };
            btnTabThang.Click += (_, __) => { dtpTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpDenNgay.Value = DateTime.Today; LoadReport(); };
        }

        private void LoadReport()
        {
            if (dtpTuNgay.Value.Date > dtpDenNgay.Value.Date) { MessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng đến ngày."); return; }
            try { _daily = _bll.GetDailyRevenue(dtpTuNgay.Value, dtpDenNgay.Value); _categories = _bll.GetCategoryRevenue(dtpTuNgay.Value, dtpDenNgay.Value); panelBarCanvas.Invalidate(); panelPieCanvas.Invalidate(); }
            catch (Exception ex) { MessageBox.Show("Không thể tải thống kê: " + ex.Message); }
        }

        private void panelBarCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int leftPadding = 45;
            int rightPadding = 20;
            int topPadding = 20;
            int bottomPadding = 35;

            int w = panelBarCanvas.Width - leftPadding - rightPadding;
            int h = panelBarCanvas.Height - topPadding - bottomPadding;

            if (w <= 0 || h <= 0) return;

            // Draw Y-Axis Gridlines & Labels
            string[] yLabels = { "15M", "10M", "5M", "0M" };
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

            // X-Axis and Bars
            string[] xLabels = _daily.ConvertAll(x => x.Date.ToString("dd/MM")).ToArray();
            float[] values = _daily.ConvertAll(x => (float)x.Revenue).ToArray();
            if (xLabels.Length == 0) { xLabels = new[] { "Không có dữ liệu" }; values = new[] { 0f }; }

            int barWidth = Math.Max(18, w / (xLabels.Length * 2));
            int stepX = w / xLabels.Length;

            using (Brush barBrush = new SolidBrush(Color.FromArgb(13, 110, 253)))
            using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                for (int i = 0; i < xLabels.Length; i++)
                {
                    int xCenter = leftPadding + (i * stepX) + (stepX / 2);
                    int barX = xCenter - (barWidth / 2);

                    float max = 1f; foreach (var value in values) if (value > max) max = value;
                    float normalizedVal = values[i] / max;
                    int barHeight = (int)(normalizedVal * h);
                    int barY = topPadding + (h - barHeight);

                    // Draw bar
                    g.FillRectangle(barBrush, barX, barY, barWidth, barHeight);

                    // Draw X Label
                    SizeF labelSize = g.MeasureString(xLabels[i], font);
                    g.DrawString(xLabels[i], font, textBrush, xCenter - (labelSize.Width / 2), topPadding + h + 10);
                }
            }
        }

        private void panelPieCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int diameter = Math.Min(panelPieCanvas.Width / 2, panelPieCanvas.Height - 60);
            if (diameter < 100) diameter = 100;

            int pieX = 30;
            int pieY = (panelPieCanvas.Height - diameter) / 2;
            Rectangle pieRect = new Rectangle(pieX, pieY, diameter, diameter);

            // Slices: Massage 40%, Giặt ủi 30%, Xông hơi 20%, Thủy liệu 10%
            float[] percentages = _categories.ConvertAll(x => x.Percentage).ToArray();
            string[] names = _categories.ConvertAll(x => x.Category + " (" + x.Percentage.ToString("0.#") + "%)").ToArray();
            if (percentages.Length == 0) { percentages = new[] { 100f }; names = new[] { "Chưa có doanh thu" }; }
            Color[] colors = {
                Color.FromArgb(0, 172, 193),   // Teal
                Color.FromArgb(13, 110, 253),  // Blue
                Color.FromArgb(245, 158, 11),  // Amber / Orange
                    Color.FromArgb(239, 68, 68), Color.MediumPurple, Color.SeaGreen, Color.SandyBrown
            };

            float startAngle = 0f;
            for (int i = 0; i < percentages.Length; i++)
            {
                float sweepAngle = (percentages[i] / 100f) * 360f;
                    using (SolidBrush brush = new SolidBrush(colors[i % colors.Length]))
                {
                    g.FillPie(brush, pieRect, startAngle, sweepAngle);
                }
                startAngle += sweepAngle;
            }

            // Legend on the right
            int legendX = pieX + diameter + 30;
            int legendY = pieY + 20;

            using (Font font = new Font("Segoe UI", 9.5F, FontStyle.Regular))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                for (int i = 0; i < names.Length; i++)
                {
                    using (SolidBrush circleBrush = new SolidBrush(colors[i % colors.Length]))
                    {
                        g.FillEllipse(circleBrush, legendX, legendY + (i * 32) + 4, 12, 12);
                    }
                    g.DrawString(names[i], font, textBrush, legendX + 20, legendY + (i * 32));
                }
            }
        }
    }
}
