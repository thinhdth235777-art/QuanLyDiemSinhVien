-- Student Management SQL Script

CREATE DATABASE QuanlyDSV;
GO
USE QuanlyDSV;
GO

-- Table: Lop
CREATE TABLE Lop (
    MaLop VARCHAR(10) PRIMARY KEY,
    TenLop NVARCHAR(50)
);
GO

-- Table: SinhVien
CREATE TABLE SinhVien (
    MaSV VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(50),
    NgaySinh DATE,
    GioiTinh BIT,
    MaLop VARCHAR(10),
    FOREIGN KEY (MaLop) REFERENCES Lop(MaLop)
);
GO

-- Table: MonHoc
CREATE TABLE MonHoc (
    MaMon VARCHAR(10) PRIMARY KEY,
    TenMon NVARCHAR(50),
    SoTinChi INT
);
GO

-- Table: Diem
CREATE TABLE Diem (
    MaSV VARCHAR(10),
    MaMon VARCHAR(10),
    DiemGK FLOAT,
    DiemCK FLOAT,
    DiemTB AS ((DiemGK + DiemCK*2)/3),
    PRIMARY KEY (MaSV, MaMon),
    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon)
);
GO

-- Sample data
INSERT INTO Lop VALUES ('CT101', N'Công nghệ thông tin 1');
INSERT INTO Lop VALUES ('CT102', N'Công nghệ thông tin 2');

INSERT INTO SinhVien VALUES ('SV001', N'Nguyễn Trần Quốc Thái', '2005-12-23', 1, 'CT101');
INSERT INTO SinhVien VALUES ('SV002', N'Trần Anh Tiến', '2005-08-14', 0, 'CT101');

INSERT INTO MonHoc VALUES ('MH01', N'Lập Trình C#', 3);
INSERT INTO MonHoc VALUES ('MH02', N'Cơ Sở Dữ Liệu', 4);

INSERT INTO Diem (MaSV, MaMon, DiemGK, DiemCK) VALUES ('SV001','MH01',7.5,8.0);
INSERT INTO Diem (MaSV, MaMon, DiemGK, DiemCK) VALUES ('SV002','MH02',8.0,7.0);
GO
