namespace DichVuKhachSan
{
    partial class FrmThongKeDoanhThu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelTopFilter = new System.Windows.Forms.Panel();
            btnThongKe = new System.Windows.Forms.Button();
            dtpDenNgay = new System.Windows.Forms.DateTimePicker();
            lblDenNgay = new System.Windows.Forms.Label();
            dtpTuNgay = new System.Windows.Forms.DateTimePicker();
            lblTuNgay = new System.Windows.Forms.Label();
            btnTabDichVu = new System.Windows.Forms.Button();
            btnTabThang = new System.Windows.Forms.Button();
            btnTabNgay = new System.Windows.Forms.Button();
            panelChartsContainer = new System.Windows.Forms.TableLayoutPanel();
            cardPieChart = new System.Windows.Forms.Panel();
            panelPieCanvas = new System.Windows.Forms.Panel();
            lblPieTitle = new System.Windows.Forms.Label();
            cardBarChart = new System.Windows.Forms.Panel();
            panelBarCanvas = new System.Windows.Forms.Panel();
            lblBarTitle = new System.Windows.Forms.Label();
            panelTopFilter.SuspendLayout();
            panelChartsContainer.SuspendLayout();
            cardPieChart.SuspendLayout();
            cardBarChart.SuspendLayout();
            SuspendLayout();
            // 
            // panelTopFilter
            // 
            panelTopFilter.BackColor = System.Drawing.Color.White;
            panelTopFilter.Controls.Add(btnThongKe);
            panelTopFilter.Controls.Add(dtpDenNgay);
            panelTopFilter.Controls.Add(lblDenNgay);
            panelTopFilter.Controls.Add(dtpTuNgay);
            panelTopFilter.Controls.Add(lblTuNgay);
            panelTopFilter.Controls.Add(btnTabDichVu);
            panelTopFilter.Controls.Add(btnTabThang);
            panelTopFilter.Controls.Add(btnTabNgay);
            panelTopFilter.Dock = System.Windows.Forms.DockStyle.Top;
            panelTopFilter.Location = new System.Drawing.Point(15, 15);
            panelTopFilter.Name = "panelTopFilter";
            panelTopFilter.Padding = new System.Windows.Forms.Padding(15);
            panelTopFilter.Size = new System.Drawing.Size(1024, 75);
            panelTopFilter.TabIndex = 0;
            // 
            // btnThongKe
            // 
            btnThongKe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnThongKe.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
            btnThongKe.Cursor = System.Windows.Forms.Cursors.Hand;
            btnThongKe.FlatAppearance.BorderSize = 0;
            btnThongKe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnThongKe.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btnThongKe.ForeColor = System.Drawing.Color.White;
            btnThongKe.Location = new System.Drawing.Point(890, 20);
            btnThongKe.Name = "btnThongKe";
            btnThongKe.Size = new System.Drawing.Size(120, 36);
            btnThongKe.TabIndex = 7;
            btnThongKe.Text = "📊  Thống kê";
            btnThongKe.UseVisualStyleBackColor = false;
            // 
            // dtpDenNgay
            // 
            dtpDenNgay.CustomFormat = "dd/MM/yyyy";
            dtpDenNgay.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dtpDenNgay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtpDenNgay.Location = new System.Drawing.Point(670, 25);
            dtpDenNgay.Name = "dtpDenNgay";
            dtpDenNgay.Size = new System.Drawing.Size(140, 24);
            dtpDenNgay.TabIndex = 6;
            // 
            // lblDenNgay
            // 
            lblDenNgay.AutoSize = true;
            lblDenNgay.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblDenNgay.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            lblDenNgay.Location = new System.Drawing.Point(600, 30);
            lblDenNgay.Name = "lblDenNgay";
            lblDenNgay.Size = new System.Drawing.Size(60, 15);
            lblDenNgay.TabIndex = 5;
            lblDenNgay.Text = "Đến ngày:";
            // 
            // dtpTuNgay
            // 
            dtpTuNgay.CustomFormat = "dd/MM/yyyy";
            dtpTuNgay.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dtpTuNgay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtpTuNgay.Location = new System.Drawing.Point(440, 25);
            dtpTuNgay.Name = "dtpTuNgay";
            dtpTuNgay.Size = new System.Drawing.Size(140, 24);
            dtpTuNgay.TabIndex = 4;
            // 
            // lblTuNgay
            // 
            lblTuNgay.AutoSize = true;
            lblTuNgay.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblTuNgay.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            lblTuNgay.Location = new System.Drawing.Point(380, 30);
            lblTuNgay.Name = "lblTuNgay";
            lblTuNgay.Size = new System.Drawing.Size(52, 15);
            lblTuNgay.TabIndex = 3;
            lblTuNgay.Text = "Từ ngày:";
            // 
            // btnTabDichVu
            // 
            btnTabDichVu.BackColor = System.Drawing.Color.White;
            btnTabDichVu.Cursor = System.Windows.Forms.Cursors.Hand;
            btnTabDichVu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(209, 213, 219);
            btnTabDichVu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnTabDichVu.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnTabDichVu.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            btnTabDichVu.Location = new System.Drawing.Point(235, 22);
            btnTabDichVu.Name = "btnTabDichVu";
            btnTabDichVu.Size = new System.Drawing.Size(105, 32);
            btnTabDichVu.TabIndex = 2;
            btnTabDichVu.Text = "Theo dịch vụ";
            btnTabDichVu.UseVisualStyleBackColor = false;
            // 
            // btnTabThang
            // 
            btnTabThang.BackColor = System.Drawing.Color.White;
            btnTabThang.Cursor = System.Windows.Forms.Cursors.Hand;
            btnTabThang.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(209, 213, 219);
            btnTabThang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnTabThang.Font = new System.Drawing.Font("Segoe UI", 9F);
            btnTabThang.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            btnTabThang.Location = new System.Drawing.Point(125, 22);
            btnTabThang.Name = "btnTabThang";
            btnTabThang.Size = new System.Drawing.Size(100, 32);
            btnTabThang.TabIndex = 1;
            btnTabThang.Text = "Theo tháng";
            btnTabThang.UseVisualStyleBackColor = false;
            // 
            // btnTabNgay
            // 
            btnTabNgay.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
            btnTabNgay.Cursor = System.Windows.Forms.Cursors.Hand;
            btnTabNgay.FlatAppearance.BorderSize = 0;
            btnTabNgay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnTabNgay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnTabNgay.ForeColor = System.Drawing.Color.White;
            btnTabNgay.Location = new System.Drawing.Point(15, 22);
            btnTabNgay.Name = "btnTabNgay";
            btnTabNgay.Size = new System.Drawing.Size(100, 32);
            btnTabNgay.TabIndex = 0;
            btnTabNgay.Text = "Theo ngày";
            btnTabNgay.UseVisualStyleBackColor = false;
            // 
            // panelChartsContainer
            // 
            panelChartsContainer.ColumnCount = 2;
            panelChartsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            panelChartsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            panelChartsContainer.Controls.Add(cardPieChart, 1, 0);
            panelChartsContainer.Controls.Add(cardBarChart, 0, 0);
            panelChartsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            panelChartsContainer.Location = new System.Drawing.Point(15, 90);
            panelChartsContainer.Name = "panelChartsContainer";
            panelChartsContainer.RowCount = 1;
            panelChartsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panelChartsContainer.Size = new System.Drawing.Size(1024, 486);
            panelChartsContainer.TabIndex = 1;
            // 
            // cardPieChart
            // 
            cardPieChart.BackColor = System.Drawing.Color.White;
            cardPieChart.Controls.Add(panelPieCanvas);
            cardPieChart.Controls.Add(lblPieTitle);
            cardPieChart.Dock = System.Windows.Forms.DockStyle.Fill;
            cardPieChart.Location = new System.Drawing.Point(571, 10);
            cardPieChart.Margin = new System.Windows.Forms.Padding(8, 10, 0, 0);
            cardPieChart.Name = "cardPieChart";
            cardPieChart.Padding = new System.Windows.Forms.Padding(15);
            cardPieChart.Size = new System.Drawing.Size(453, 476);
            cardPieChart.TabIndex = 1;
            // 
            // panelPieCanvas
            // 
            panelPieCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            panelPieCanvas.Location = new System.Drawing.Point(15, 36);
            panelPieCanvas.Name = "panelPieCanvas";
            panelPieCanvas.Size = new System.Drawing.Size(423, 425);
            panelPieCanvas.TabIndex = 2;
            panelPieCanvas.Paint += panelPieCanvas_Paint;
            // 
            // lblPieTitle
            // 
            lblPieTitle.AutoSize = true;
            lblPieTitle.Dock = System.Windows.Forms.DockStyle.Top;
            lblPieTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            lblPieTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            lblPieTitle.Location = new System.Drawing.Point(15, 15);
            lblPieTitle.Name = "lblPieTitle";
            lblPieTitle.Size = new System.Drawing.Size(190, 21);
            lblPieTitle.TabIndex = 1;
            lblPieTitle.Text = "Doanh thu theo dịch vụ";
            // 
            // cardBarChart
            // 
            cardBarChart.BackColor = System.Drawing.Color.White;
            cardBarChart.Controls.Add(panelBarCanvas);
            cardBarChart.Controls.Add(lblBarTitle);
            cardBarChart.Dock = System.Windows.Forms.DockStyle.Fill;
            cardBarChart.Location = new System.Drawing.Point(0, 10);
            cardBarChart.Margin = new System.Windows.Forms.Padding(0, 10, 8, 0);
            cardBarChart.Name = "cardBarChart";
            cardBarChart.Padding = new System.Windows.Forms.Padding(15);
            cardBarChart.Size = new System.Drawing.Size(555, 476);
            cardBarChart.TabIndex = 0;
            // 
            // panelBarCanvas
            // 
            panelBarCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            panelBarCanvas.Location = new System.Drawing.Point(15, 36);
            panelBarCanvas.Name = "panelBarCanvas";
            panelBarCanvas.Size = new System.Drawing.Size(525, 425);
            panelBarCanvas.TabIndex = 1;
            panelBarCanvas.Paint += panelBarCanvas_Paint;
            // 
            // lblBarTitle
            // 
            lblBarTitle.AutoSize = true;
            lblBarTitle.Dock = System.Windows.Forms.DockStyle.Top;
            lblBarTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            lblBarTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            lblBarTitle.Location = new System.Drawing.Point(15, 15);
            lblBarTitle.Name = "lblBarTitle";
            lblBarTitle.Size = new System.Drawing.Size(172, 21);
            lblBarTitle.TabIndex = 0;
            lblBarTitle.Text = "Doanh thu theo ngày";
            // 
            // FrmThongKeDoanhThu
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            ClientSize = new System.Drawing.Size(1054, 591);
            Controls.Add(panelChartsContainer);
            Controls.Add(panelTopFilter);
            Font = new System.Drawing.Font("Segoe UI", 9.5F);
            Name = "FrmThongKeDoanhThu";
            Padding = new System.Windows.Forms.Padding(15);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Thống kê doanh thu";
            panelTopFilter.ResumeLayout(false);
            panelTopFilter.PerformLayout();
            panelChartsContainer.ResumeLayout(false);
            cardPieChart.ResumeLayout(false);
            cardPieChart.PerformLayout();
            cardBarChart.ResumeLayout(false);
            cardBarChart.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTopFilter;
        private System.Windows.Forms.Button btnTabNgay;
        private System.Windows.Forms.Button btnTabThang;
        private System.Windows.Forms.Button btnTabDichVu;
        private System.Windows.Forms.Label lblTuNgay;
        private System.Windows.Forms.DateTimePicker dtpTuNgay;
        private System.Windows.Forms.Label lblDenNgay;
        private System.Windows.Forms.DateTimePicker dtpDenNgay;
        private System.Windows.Forms.Button btnThongKe;
        private System.Windows.Forms.TableLayoutPanel panelChartsContainer;
        private System.Windows.Forms.Panel cardBarChart;
        private System.Windows.Forms.Label lblBarTitle;
        private System.Windows.Forms.Panel panelBarCanvas;
        private System.Windows.Forms.Panel cardPieChart;
        private System.Windows.Forms.Label lblPieTitle;
        private System.Windows.Forms.Panel panelPieCanvas;
    }
}
