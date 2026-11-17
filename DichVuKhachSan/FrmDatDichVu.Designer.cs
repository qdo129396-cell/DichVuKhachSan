namespace DichVuKhachSan
{
    partial class FrmDatDichVu
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnDatDichVu = new System.Windows.Forms.Button();
            this.txtGhiChu = new System.Windows.Forms.TextBox();
            this.lblGhiChu = new System.Windows.Forms.Label();
            this.dtpNgaySuDung = new System.Windows.Forms.DateTimePicker();
            this.lblNgaySuDung = new System.Windows.Forms.Label();
            this.txtSoLuong = new System.Windows.Forms.TextBox();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.cboDichVu = new System.Windows.Forms.ComboBox();
            this.lblDichVu = new System.Windows.Forms.Label();
            this.cboKhachHang = new System.Windows.Forms.ComboBox();
            this.lblKhachHang = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.lblThanhTienVal = new System.Windows.Forms.Label();
            this.lblThanhTienTitle = new System.Windows.Forms.Label();
            this.panelDivider = new System.Windows.Forms.Panel();
            this.lblPreviewSoLuongVal = new System.Windows.Forms.Label();
            this.lblPreviewSoLuongTitle = new System.Windows.Forms.Label();
            this.lblPreviewDonGiaVal = new System.Windows.Forms.Label();
            this.lblPreviewDonGiaTitle = new System.Windows.Forms.Label();
            this.lblPreviewDichVuVal = new System.Windows.Forms.Label();
            this.lblPreviewDichVuTitle = new System.Windows.Forms.Label();
            this.lblSummaryTitle = new System.Windows.Forms.Label();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.White;
            this.panelLeft.Controls.Add(this.btnDatDichVu);
            this.panelLeft.Controls.Add(this.txtGhiChu);
            this.panelLeft.Controls.Add(this.lblGhiChu);
            this.panelLeft.Controls.Add(this.dtpNgaySuDung);
            this.panelLeft.Controls.Add(this.lblNgaySuDung);
            this.panelLeft.Controls.Add(this.txtSoLuong);
            this.panelLeft.Controls.Add(this.lblSoLuong);
            this.panelLeft.Controls.Add(this.cboDichVu);
            this.panelLeft.Controls.Add(this.lblDichVu);
            this.panelLeft.Controls.Add(this.cboKhachHang);
            this.panelLeft.Controls.Add(this.lblKhachHang);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(15, 15);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(20);
            this.panelLeft.Size = new System.Drawing.Size(530, 451);
            this.panelLeft.TabIndex = 0;
            // 
            // btnDatDichVu
            // 
            this.btnDatDichVu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.btnDatDichVu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDatDichVu.FlatAppearance.BorderSize = 0;
            this.btnDatDichVu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDatDichVu.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDatDichVu.ForeColor = System.Drawing.Color.White;
            this.btnDatDichVu.Location = new System.Drawing.Point(125, 365);
            this.btnDatDichVu.Name = "btnDatDichVu";
            this.btnDatDichVu.Size = new System.Drawing.Size(370, 42);
            this.btnDatDichVu.TabIndex = 10;
            this.btnDatDichVu.Text = "✓  Đặt dịch vụ";
            this.btnDatDichVu.UseVisualStyleBackColor = false;
            // 
            // txtGhiChu
            // 
            this.txtGhiChu.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtGhiChu.Location = new System.Drawing.Point(125, 220);
            this.txtGhiChu.Multiline = true;
            this.txtGhiChu.Name = "txtGhiChu";
            this.txtGhiChu.Size = new System.Drawing.Size(370, 110);
            this.txtGhiChu.TabIndex = 9;
            this.txtGhiChu.Text = "Thư giãn sau chuyến đi";
            // 
            // lblGhiChu
            // 
            this.lblGhiChu.AutoSize = true;
            this.lblGhiChu.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblGhiChu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblGhiChu.Location = new System.Drawing.Point(20, 225);
            this.lblGhiChu.Name = "lblGhiChu";
            this.lblGhiChu.Size = new System.Drawing.Size(54, 17);
            this.lblGhiChu.TabIndex = 8;
            this.lblGhiChu.Text = "Ghi chú:";
            // 
            // dtpNgaySuDung
            // 
            this.dtpNgaySuDung.CustomFormat = "dd/MM/yyyy";
            this.dtpNgaySuDung.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dtpNgaySuDung.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgaySuDung.Location = new System.Drawing.Point(125, 170);
            this.dtpNgaySuDung.Name = "dtpNgaySuDung";
            this.dtpNgaySuDung.Size = new System.Drawing.Size(370, 24);
            this.dtpNgaySuDung.TabIndex = 7;
            // 
            // lblNgaySuDung
            // 
            this.lblNgaySuDung.AutoSize = true;
            this.lblNgaySuDung.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNgaySuDung.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblNgaySuDung.Location = new System.Drawing.Point(20, 175);
            this.lblNgaySuDung.Name = "lblNgaySuDung";
            this.lblNgaySuDung.Size = new System.Drawing.Size(93, 17);
            this.lblNgaySuDung.TabIndex = 6;
            this.lblNgaySuDung.Text = "Ngày sử dụng:";
            // 
            // txtSoLuong
            // 
            this.txtSoLuong.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSoLuong.Location = new System.Drawing.Point(125, 120);
            this.txtSoLuong.Name = "txtSoLuong";
            this.txtSoLuong.Size = new System.Drawing.Size(370, 24);
            this.txtSoLuong.TabIndex = 5;
            this.txtSoLuong.Text = "1";
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSoLuong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblSoLuong.Location = new System.Drawing.Point(20, 125);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(64, 17);
            this.lblSoLuong.TabIndex = 4;
            this.lblSoLuong.Text = "Số lượng:";
            // 
            // cboDichVu
            // 
            this.cboDichVu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDichVu.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboDichVu.FormattingEnabled = true;
            this.cboDichVu.Items.AddRange(new object[] {
            "Massage",
            "Giặt ủi",
            "Xông hơi",
            "Thủy liệu",
            "Nước uống"});
            this.cboDichVu.Location = new System.Drawing.Point(125, 70);
            this.cboDichVu.Name = "cboDichVu";
            this.cboDichVu.Size = new System.Drawing.Size(370, 25);
            this.cboDichVu.TabIndex = 3;
            // 
            // lblDichVu
            // 
            this.lblDichVu.AutoSize = true;
            this.lblDichVu.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDichVu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblDichVu.Location = new System.Drawing.Point(20, 75);
            this.lblDichVu.Name = "lblDichVu";
            this.lblDichVu.Size = new System.Drawing.Size(53, 17);
            this.lblDichVu.TabIndex = 2;
            this.lblDichVu.Text = "Dịch vụ:";
            // 
            // cboKhachHang
            // 
            this.cboKhachHang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhachHang.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboKhachHang.FormattingEnabled = true;
            this.cboKhachHang.Items.AddRange(new object[] {
            "Nguyễn Văn An",
            "Trần Thị Bình",
            "Lê Văn Cường"});
            this.cboKhachHang.Location = new System.Drawing.Point(125, 20);
            this.cboKhachHang.Name = "cboKhachHang";
            this.cboKhachHang.Size = new System.Drawing.Size(370, 25);
            this.cboKhachHang.TabIndex = 1;
            // 
            // lblKhachHang
            // 
            this.lblKhachHang.AutoSize = true;
            this.lblKhachHang.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblKhachHang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblKhachHang.Location = new System.Drawing.Point(20, 25);
            this.lblKhachHang.Name = "lblKhachHang";
            this.lblKhachHang.Size = new System.Drawing.Size(79, 17);
            this.lblKhachHang.TabIndex = 0;
            this.lblKhachHang.Text = "Khách hàng:";
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.White;
            this.panelRight.Controls.Add(this.lblThanhTienVal);
            this.panelRight.Controls.Add(this.lblThanhTienTitle);
            this.panelRight.Controls.Add(this.panelDivider);
            this.panelRight.Controls.Add(this.lblPreviewSoLuongVal);
            this.panelRight.Controls.Add(this.lblPreviewSoLuongTitle);
            this.panelRight.Controls.Add(this.lblPreviewDonGiaVal);
            this.panelRight.Controls.Add(this.lblPreviewDonGiaTitle);
            this.panelRight.Controls.Add(this.lblPreviewDichVuVal);
            this.panelRight.Controls.Add(this.lblPreviewDichVuTitle);
            this.panelRight.Controls.Add(this.lblSummaryTitle);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(545, 15);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(20);
            this.panelRight.Size = new System.Drawing.Size(364, 451);
            this.panelRight.TabIndex = 1;
            // 
            // lblThanhTienVal
            // 
            this.lblThanhTienVal.AutoSize = true;
            this.lblThanhTienVal.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblThanhTienVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblThanhTienVal.Location = new System.Drawing.Point(125, 235);
            this.lblThanhTienVal.Name = "lblThanhTienVal";
            this.lblThanhTienVal.Size = new System.Drawing.Size(126, 25);
            this.lblThanhTienVal.TabIndex = 9;
            this.lblThanhTienVal.Text = "150,000 VNĐ";
            // 
            // lblThanhTienTitle
            // 
            this.lblThanhTienTitle.AutoSize = true;
            this.lblThanhTienTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblThanhTienTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblThanhTienTitle.Location = new System.Drawing.Point(20, 240);
            this.lblThanhTienTitle.Name = "lblThanhTienTitle";
            this.lblThanhTienTitle.Size = new System.Drawing.Size(84, 19);
            this.lblThanhTienTitle.TabIndex = 8;
            this.lblThanhTienTitle.Text = "Thành tiền:";
            // 
            // panelDivider
            // 
            this.panelDivider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.panelDivider.Location = new System.Drawing.Point(20, 205);
            this.panelDivider.Name = "panelDivider";
            this.panelDivider.Size = new System.Drawing.Size(320, 1);
            this.panelDivider.TabIndex = 7;
            // 
            // lblPreviewSoLuongVal
            // 
            this.lblPreviewSoLuongVal.AutoSize = true;
            this.lblPreviewSoLuongVal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPreviewSoLuongVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblPreviewSoLuongVal.Location = new System.Drawing.Point(125, 160);
            this.lblPreviewSoLuongVal.Name = "lblPreviewSoLuongVal";
            this.lblPreviewSoLuongVal.Size = new System.Drawing.Size(15, 17);
            this.lblPreviewSoLuongVal.TabIndex = 6;
            this.lblPreviewSoLuongVal.Text = "1";
            // 
            // lblPreviewSoLuongTitle
            // 
            this.lblPreviewSoLuongTitle.AutoSize = true;
            this.lblPreviewSoLuongTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPreviewSoLuongTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblPreviewSoLuongTitle.Location = new System.Drawing.Point(20, 160);
            this.lblPreviewSoLuongTitle.Name = "lblPreviewSoLuongTitle";
            this.lblPreviewSoLuongTitle.Size = new System.Drawing.Size(64, 17);
            this.lblPreviewSoLuongTitle.TabIndex = 5;
            this.lblPreviewSoLuongTitle.Text = "Số lượng:";
            // 
            // lblPreviewDonGiaVal
            // 
            this.lblPreviewDonGiaVal.AutoSize = true;
            this.lblPreviewDonGiaVal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPreviewDonGiaVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblPreviewDonGiaVal.Location = new System.Drawing.Point(125, 115);
            this.lblPreviewDonGiaVal.Name = "lblPreviewDonGiaVal";
            this.lblPreviewDonGiaVal.Size = new System.Drawing.Size(83, 17);
            this.lblPreviewDonGiaVal.TabIndex = 4;
            this.lblPreviewDonGiaVal.Text = "150,000 VNĐ";
            // 
            // lblPreviewDonGiaTitle
            // 
            this.lblPreviewDonGiaTitle.AutoSize = true;
            this.lblPreviewDonGiaTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPreviewDonGiaTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblPreviewDonGiaTitle.Location = new System.Drawing.Point(20, 115);
            this.lblPreviewDonGiaTitle.Name = "lblPreviewDonGiaTitle";
            this.lblPreviewDonGiaTitle.Size = new System.Drawing.Size(57, 17);
            this.lblPreviewDonGiaTitle.TabIndex = 3;
            this.lblPreviewDonGiaTitle.Text = "Đơn giá:";
            // 
            // lblPreviewDichVuVal
            // 
            this.lblPreviewDichVuVal.AutoSize = true;
            this.lblPreviewDichVuVal.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPreviewDichVuVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblPreviewDichVuVal.Location = new System.Drawing.Point(125, 70);
            this.lblPreviewDichVuVal.Name = "lblPreviewDichVuVal";
            this.lblPreviewDichVuVal.Size = new System.Drawing.Size(60, 17);
            this.lblPreviewDichVuVal.TabIndex = 2;
            this.lblPreviewDichVuVal.Text = "Massage";
            // 
            // lblPreviewDichVuTitle
            // 
            this.lblPreviewDichVuTitle.AutoSize = true;
            this.lblPreviewDichVuTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPreviewDichVuTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblPreviewDichVuTitle.Location = new System.Drawing.Point(20, 70);
            this.lblPreviewDichVuTitle.Name = "lblPreviewDichVuTitle";
            this.lblPreviewDichVuTitle.Size = new System.Drawing.Size(53, 17);
            this.lblPreviewDichVuTitle.TabIndex = 1;
            this.lblPreviewDichVuTitle.Text = "Dịch vụ:";
            // 
            // lblSummaryTitle
            // 
            this.lblSummaryTitle.AutoSize = true;
            this.lblSummaryTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSummaryTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblSummaryTitle.Location = new System.Drawing.Point(20, 20);
            this.lblSummaryTitle.Name = "lblSummaryTitle";
            this.lblSummaryTitle.Size = new System.Drawing.Size(177, 21);
            this.lblSummaryTitle.TabIndex = 0;
            this.lblSummaryTitle.Text = "Thông tin đặt dịch vụ";
            // 
            // FrmDatDichVu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(924, 481);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "FrmDatDichVu";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đặt dịch vụ";
            this.Load += new System.EventHandler(this.FrmDatDichVu_Load);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label lblKhachHang;
        private System.Windows.Forms.ComboBox cboKhachHang;
        private System.Windows.Forms.Label lblDichVu;
        private System.Windows.Forms.ComboBox cboDichVu;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.TextBox txtSoLuong;
        private System.Windows.Forms.Label lblNgaySuDung;
        private System.Windows.Forms.DateTimePicker dtpNgaySuDung;
        private System.Windows.Forms.Label lblGhiChu;
        private System.Windows.Forms.TextBox txtGhiChu;
        private System.Windows.Forms.Button btnDatDichVu;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Label lblSummaryTitle;
        private System.Windows.Forms.Label lblPreviewDichVuTitle;
        private System.Windows.Forms.Label lblPreviewDichVuVal;
        private System.Windows.Forms.Label lblPreviewDonGiaTitle;
        private System.Windows.Forms.Label lblPreviewDonGiaVal;
        private System.Windows.Forms.Label lblPreviewSoLuongTitle;
        private System.Windows.Forms.Label lblPreviewSoLuongVal;
        private System.Windows.Forms.Panel panelDivider;
        private System.Windows.Forms.Label lblThanhTienTitle;
        private System.Windows.Forms.Label lblThanhTienVal;
    }
}
