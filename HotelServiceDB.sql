/*
    HotelServiceDB - SQL Server
    Database for HotelService Windows Forms application.
    Run this script in SQL Server Management Studio (SSMS).
    It creates the database, tables, constraints, indexes and sample data.

    NOTE:
    - This project manages hotel SERVICES only; it does not manage rooms.
    - RoomNumber is stored on a service order as a reference entered by staff.
    - No default login account is created. Create the first administrator
      through the application's first-run/setup flow and store a PBKDF2 hash.
*/

USE master;
GO

IF DB_ID(N'HotelServiceDB') IS NULL
BEGIN
    CREATE DATABASE HotelServiceDB;
END;
GO

USE HotelServiceDB;
GO

/* USERS */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username        NVARCHAR(50) NOT NULL,
        FullName        NVARCHAR(120) NOT NULL,
        PasswordHash    NVARCHAR(500) NOT NULL,
        Role            NVARCHAR(20) NOT NULL CONSTRAINT DF_Users_Role DEFAULT N'Staff',
        IsActive        BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT CK_Users_Role CHECK (Role IN (N'Admin', N'Manager', N'Staff'))
    );
END;
GO

/* CUSTOMERS */
IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers
    (
        CustomerId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
        CustomerCode    NVARCHAR(20) NOT NULL,
        FullName        NVARCHAR(120) NOT NULL,
        Phone           NVARCHAR(20) NULL,
        Email           NVARCHAR(120) NULL,
        Address         NVARCHAR(250) NULL,
        Notes           NVARCHAR(500) NULL,
        IsActive        BIT NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT 1,
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT SYSDATETIME(),
        CONSTRAINT UQ_Customers_CustomerCode UNIQUE (CustomerCode)
    );
END;
GO

/* SERVICES */
IF OBJECT_ID(N'dbo.Services', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Services
    (
        ServiceId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Services PRIMARY KEY,
        ServiceCode     NVARCHAR(20) NOT NULL,
        ServiceName     NVARCHAR(120) NOT NULL,
        Category        NVARCHAR(50) NOT NULL,
        Unit            NVARCHAR(30) NOT NULL CONSTRAINT DF_Services_Unit DEFAULT N'Lần',
        UnitPrice       DECIMAL(18,2) NOT NULL,
        Description     NVARCHAR(500) NULL,
        IsActive        BIT NOT NULL CONSTRAINT DF_Services_IsActive DEFAULT 1,
        CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Services_CreatedAt DEFAULT SYSDATETIME(),
        CONSTRAINT UQ_Services_ServiceCode UNIQUE (ServiceCode),
        CONSTRAINT CK_Services_UnitPrice CHECK (UnitPrice >= 0)
    );
END;
GO

/* SERVICE ORDERS */
IF OBJECT_ID(N'dbo.ServiceOrders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceOrders
    (
        OrderId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ServiceOrders PRIMARY KEY,
        OrderCode       NVARCHAR(30) NOT NULL,
        CustomerId      INT NULL,
        RoomNumber      NVARCHAR(20) NULL,
        CreatedBy       INT NOT NULL,
        OrderDate       DATETIME2(0) NOT NULL CONSTRAINT DF_ServiceOrders_OrderDate DEFAULT SYSDATETIME(),
        Status          NVARCHAR(20) NOT NULL CONSTRAINT DF_ServiceOrders_Status DEFAULT N'Pending',
        Notes           NVARCHAR(500) NULL,
        CONSTRAINT UQ_ServiceOrders_OrderCode UNIQUE (OrderCode),
        CONSTRAINT FK_ServiceOrders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
        CONSTRAINT FK_ServiceOrders_Users FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
        CONSTRAINT CK_ServiceOrders_Status CHECK (Status IN (N'Pending', N'Processing', N'Completed', N'Cancelled'))
    );
END;
GO

/* SERVICE ORDER DETAILS */
IF OBJECT_ID(N'dbo.ServiceOrderDetails', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceOrderDetails
    (
        OrderDetailId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ServiceOrderDetails PRIMARY KEY,
        OrderId         INT NOT NULL,
        ServiceId       INT NOT NULL,
        Quantity        DECIMAL(12,2) NOT NULL,
        UnitPrice       DECIMAL(18,2) NOT NULL,
        LineTotal       AS CONVERT(DECIMAL(18,2), Quantity * UnitPrice) PERSISTED,
        Notes           NVARCHAR(250) NULL,
        CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId) REFERENCES dbo.ServiceOrders(OrderId) ON DELETE CASCADE,
        CONSTRAINT FK_OrderDetails_Services FOREIGN KEY (ServiceId) REFERENCES dbo.Services(ServiceId),
        CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_OrderDetails_UnitPrice CHECK (UnitPrice >= 0)
    );
END;
GO

/* INVOICES: one invoice per service order */
IF OBJECT_ID(N'dbo.Invoices', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Invoices
    (
        InvoiceId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Invoices PRIMARY KEY,
        InvoiceCode     NVARCHAR(30) NOT NULL,
        OrderId         INT NOT NULL,
        IssuedBy        INT NOT NULL,
        InvoiceDate     DATETIME2(0) NOT NULL CONSTRAINT DF_Invoices_InvoiceDate DEFAULT SYSDATETIME(),
        DiscountAmount  DECIMAL(18,2) NOT NULL CONSTRAINT DF_Invoices_Discount DEFAULT 0,
        TotalAmount     DECIMAL(18,2) NOT NULL,
        PaymentStatus   NVARCHAR(20) NOT NULL CONSTRAINT DF_Invoices_PaymentStatus DEFAULT N'Unpaid',
        PaymentMethod   NVARCHAR(30) NULL,
        PaidAt          DATETIME2(0) NULL,
        Notes           NVARCHAR(500) NULL,
        CONSTRAINT UQ_Invoices_InvoiceCode UNIQUE (InvoiceCode),
        CONSTRAINT UQ_Invoices_OrderId UNIQUE (OrderId),
        CONSTRAINT FK_Invoices_Orders FOREIGN KEY (OrderId) REFERENCES dbo.ServiceOrders(OrderId),
        CONSTRAINT FK_Invoices_Users FOREIGN KEY (IssuedBy) REFERENCES dbo.Users(UserId),
        CONSTRAINT CK_Invoices_Discount CHECK (DiscountAmount >= 0),
        CONSTRAINT CK_Invoices_Total CHECK (TotalAmount >= 0),
        CONSTRAINT CK_Invoices_PaymentStatus CHECK (PaymentStatus IN (N'Unpaid', N'Paid', N'Cancelled'))
    );
END;
GO

/* Helpful indexes */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceOrders_OrderDate' AND object_id = OBJECT_ID(N'dbo.ServiceOrders'))
    CREATE INDEX IX_ServiceOrders_OrderDate ON dbo.ServiceOrders(OrderDate);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ServiceOrders_CustomerId' AND object_id = OBJECT_ID(N'dbo.ServiceOrders'))
    CREATE INDEX IX_ServiceOrders_CustomerId ON dbo.ServiceOrders(CustomerId);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Invoices_InvoiceDate' AND object_id = OBJECT_ID(N'dbo.Invoices'))
    CREATE INDEX IX_Invoices_InvoiceDate ON dbo.Invoices(InvoiceDate);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Invoices_PaymentStatus' AND object_id = OBJECT_ID(N'dbo.Invoices'))
    CREATE INDEX IX_Invoices_PaymentStatus ON dbo.Invoices(PaymentStatus);
GO

/* Sample service catalog. Insert only when the catalog is empty. */
IF NOT EXISTS (SELECT 1 FROM dbo.Services)
BEGIN
    INSERT INTO dbo.Services (ServiceCode, ServiceName, Category, Unit, UnitPrice, Description)
    VALUES
    (N'DV001', N'Giặt ủi quần áo', N'Giặt ủi', N'Kg', 30000, N'Dịch vụ giặt và ủi quần áo'),
    (N'DV002', N'Nước suối', N'Đồ uống', N'Chai', 15000, N'Nước suối đóng chai'),
    (N'DV003', N'Massage thư giãn', N'Spa', N'Giờ', 250000, N'Dịch vụ massage thư giãn'),
    (N'DV004', N'Xông hơi', N'Spa', N'Lượt', 180000, N'Dịch vụ xông hơi');
END;
GO

/* Optional demo customer. Remove this block if a clean database is preferred. */
IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerCode = N'KH001')
BEGIN
    INSERT INTO dbo.Customers (CustomerCode, FullName, Phone, Email, Address, Notes)
    VALUES (N'KH001', N'Khách hàng mẫu', N'0900000000', NULL, NULL, N'Dữ liệu mẫu - có thể chỉnh sửa hoặc xóa');
END;
GO

/* Daily revenue view: counts paid invoices only. */
CREATE OR ALTER VIEW dbo.vw_DailyRevenue
AS
    SELECT
        CONVERT(DATE, InvoiceDate) AS RevenueDate,
        COUNT(*) AS InvoiceCount,
        SUM(TotalAmount - DiscountAmount) AS Revenue
    FROM dbo.Invoices
    WHERE PaymentStatus = N'Paid'
    GROUP BY CONVERT(DATE, InvoiceDate);
GO

/* Order detail view for displaying service orders and their totals. */
CREATE OR ALTER VIEW dbo.vw_ServiceOrderSummary
AS
    SELECT
        o.OrderId,
        o.OrderCode,
        o.OrderDate,
        o.RoomNumber,
        o.Status,
        c.FullName AS CustomerName,
        u.FullName AS CreatedByName,
        COUNT(d.OrderDetailId) AS ItemCount,
        COALESCE(SUM(d.LineTotal), 0) AS OrderTotal
    FROM dbo.ServiceOrders o
    LEFT JOIN dbo.Customers c ON c.CustomerId = o.CustomerId
    INNER JOIN dbo.Users u ON u.UserId = o.CreatedBy
    LEFT JOIN dbo.ServiceOrderDetails d ON d.OrderId = o.OrderId
    GROUP BY
        o.OrderId, o.OrderCode, o.OrderDate, o.RoomNumber, o.Status,
        c.FullName, u.FullName;
GO

PRINT N'HotelServiceDB đã được tạo/cập nhật thành công.';
PRINT N'Hãy tạo tài khoản Admin đầu tiên từ chức năng thiết lập tài khoản của ứng dụng.';
GO

/* Unicode repair block: safe to run repeatedly. */
UPDATE dbo.Services
SET ServiceName = N'Giặt ủi quần áo', Category = N'Giặt ủi', Unit = N'Kg',
    Description = N'Dịch vụ giặt và ủi quần áo'
WHERE ServiceCode = N'DV001';

UPDATE dbo.Services
SET ServiceName = N'Nước suối', Category = N'Đồ uống', Unit = N'Chai',
    Description = N'Nước suối đóng chai'
WHERE ServiceCode = N'DV002';

UPDATE dbo.Services
SET ServiceName = N'Massage thư giãn', Category = N'Spa', Unit = N'Giờ',
    Description = N'Dịch vụ massage thư giãn'
WHERE ServiceCode = N'DV003';

UPDATE dbo.Services
SET ServiceName = N'Xông hơi', Category = N'Spa', Unit = N'Lượt',
    Description = N'Dịch vụ xông hơi'
WHERE ServiceCode = N'DV004';

UPDATE dbo.Customers
SET FullName = N'Khách hàng mẫu', Notes = N'Dữ liệu mẫu - có thể chỉnh sửa hoặc xóa'
WHERE CustomerCode = N'KH001';
GO

/* Additional customer records. Existing customer codes are never overwritten. */
INSERT INTO dbo.Customers (CustomerCode, FullName, Phone, Email, Address, Notes)
SELECT v.CustomerCode, v.FullName, v.Phone, v.Email, v.Address, N'Dữ liệu khởi tạo'
FROM (VALUES
    (N'KH003', N'Nguyễn Minh Anh', N'0901000003', N'minh.anh@example.com', N'Quận 1, TP. Hồ Chí Minh'),
    (N'KH004', N'Trần Hoài Nam', N'0901000004', N'hoai.nam@example.com', N'Quận 3, TP. Hồ Chí Minh'),
    (N'KH005', N'Lê Thu Hà', N'0901000005', N'thu.ha@example.com', N'Hải Châu, Đà Nẵng'),
    (N'KH006', N'Phạm Quốc Huy', N'0901000006', N'quoc.huy@example.com', N'Ninh Kiều, Cần Thơ'),
    (N'KH007', N'Võ Ngọc Lan', N'0901000007', N'ngoc.lan@example.com', N'Đà Lạt, Lâm Đồng'),
    (N'KH008', N'Đặng Gia Bảo', N'0901000008', N'gia.bao@example.com', N'Biên Hòa, Đồng Nai'),
    (N'KH009', N'Bùi Thanh Thảo', N'0901000009', N'thanh.thao@example.com', N'Huế, Thừa Thiên Huế'),
    (N'KH010', N'Hoàng Đức Long', N'0901000010', N'duc.long@example.com', N'Vũng Tàu, Bà Rịa - Vũng Tàu'),
    (N'KH011', N'Phan Khánh Linh', N'0901000011', N'khanh.linh@example.com', N'Quận 7, TP. Hồ Chí Minh'),
    (N'KH012', N'Nguyễn Hải Đăng', N'0901000012', N'hai.dang@example.com', N'Thanh Xuân, Hà Nội'),
    (N'KH013', N'Trương Mỹ Duyên', N'0901000013', N'my.duyen@example.com', N'Sơn Trà, Đà Nẵng'),
    (N'KH014', N'Đỗ Thành Công', N'0901000014', N'thanh.cong@example.com', N'Hạ Long, Quảng Ninh'),
    (N'KH015', N'Ngô Bảo Trâm', N'0901000015', N'bao.tram@example.com', N'Buôn Ma Thuột, Đắk Lắk'),
    (N'KH016', N'Vũ Nhật Minh', N'0901000016', N'nhat.minh@example.com', N'Nam Từ Liêm, Hà Nội'),
    (N'KH017', N'Lý Quang Vinh', N'0901000017', N'quang.vinh@example.com', N'Rạch Giá, Kiên Giang'),
    (N'KH018', N'Phạm Diễm My', N'0901000018', N'diem.my@example.com', N'Quy Nhơn, Bình Định'),
    (N'KH019', N'Trần Quốc Khánh', N'0901000019', N'quoc.khanh@example.com', N'Long Xuyên, An Giang'),
    (N'KH020', N'Nguyễn Thùy Dương', N'0901000020', N'thuy.duong@example.com', N'Đồng Hới, Quảng Bình'),
    (N'KH021', N'Lê Anh Tuấn', N'0901000021', N'anh.tuan@example.com', N'Phan Thiết, Bình Thuận'),
    (N'KH022', N'Đinh Khả Vy', N'0901000022', N'kha.vy@example.com', N'Việt Trì, Phú Thọ')
) AS v(CustomerCode, FullName, Phone, Email, Address)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Customers c WHERE c.CustomerCode = v.CustomerCode);
GO
