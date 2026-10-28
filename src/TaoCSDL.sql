-- Chạy đoạn code này trong SQL Server Management Studio (SSMS)
CREATE DATABASE ql_sinhvien;
GO
USE ql_sinhvien;
GO

-- Bảng cho Bài 4.1
CREATE TABLE SinhVien (
    MaSV VARCHAR(20) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    Lop NVARCHAR(50),
    DiemTB DECIMAL(4,2)
);
INSERT INTO SinhVien VALUES ('SV001', N'Nguyễn Văn An', '2003-05-15', N'Nam', N'CNTT K18', 8.50);

-- Bảng cho Bài 4.2
CREATE TABLE HocVien (
    MaHV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SDT VARCHAR(20),
    NgaySinh DATE,
    KhoaHoc NVARCHAR(50),
    GioiTinh NVARCHAR(10),
    SoThich NVARCHAR(200)
);