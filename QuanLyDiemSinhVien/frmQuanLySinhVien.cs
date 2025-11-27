using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace QuanLyDiemSinhVien
{
    public partial class frmQuanLySinhVien : Form
    {
        SqlConnection conn = new SqlConnection(
        @"Data Source=.;Initial Catalog=QuanlyDSV;Integrated Security=True");
        DataSet ds = new DataSet("dsQLNV");
        SqlDataAdapter daLop;
        SqlDataAdapter daMon;
        SqlDataAdapter daDSLop;
        SqlDataAdapter daSV;
        SqlCommandBuilder cmbSV;
        SqlCommandBuilder cbMonhoc1;
        

        bool dangThemSV = false;
        bool dangThemMH = false;

        SqlDataAdapter daTraCuu;
        //....
        public frmQuanLySinhVien()
        {
            InitializeComponent();
        }
        private void frmQuanLySinhVien_Load(object sender, EventArgs e)
        {
            conn.ConnectionString = @"Data Source=.;Initial Catalog=QuanlyDSV;Integrated Security=True";
            //=========================Xử lý load Sinh Viên=========================================
            //=========================Xử lý load Môn Học===========================================
            //...
            //=========================Xử lý load Lớp Học===========================================
            LoadComboLop();
            LoadComboMon();
            LoadDanhSachLop();
            LoadSinhVien();
            KhoaLopHoc(true);
            //=========================Xử lý load Nhập Điểm=========================================
            LoadComboLop_NhapDiem();
            LoadComboMon_NhapDiem();
            LoadNhapDiem();
            LoadBangDiem();
            KhoaNhapDiem(true);
        }

        //==========================================Sinh Viên================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA SINH VIÊN:
        private void KhoaSV(bool daKhoa)
        {

            txtMaSV.ReadOnly = daKhoa || !dangThemSV;
            txtHoTen.Enabled = !daKhoa;

            dtpNgaySinh.Enabled = !daKhoa;

            chkNam.Enabled = !daKhoa;
            chkNu.Enabled = !daKhoa;
            cbLop.Enabled = !daKhoa;


            btnLuu.Enabled = !daKhoa;
            btnHuy.Enabled = !daKhoa;

            btnThem.Enabled = daKhoa;
            btnSua.Enabled = daKhoa;
            btnXoa.Enabled = daKhoa;
            btnTim.Enabled = daKhoa;
        }
        private void LoadSinhVien()
        {
            string sqlSV = "SELECT MaSV, HoTen, NgaySinh, GioiTinh, MaLop FROM SinhVien";
            daSV = new SqlDataAdapter(sqlSV, conn);
            cmbSV = new SqlCommandBuilder(daSV);
            try
            {
                if (ds.Tables.Contains("tblSV"))
                    ds.Tables["tblSV"].Clear();
                daSV.Fill(ds, "tblSV"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu Sinh Viên: " + ex.Message);
            }

            dgvSinhVien.DataSource = ds.Tables["tblSV"];
            dgvSinhVien.AutoGenerateColumns = true;

            dgvSinhVien.Columns["MaSV"].HeaderText = "Mã SV";
            dgvSinhVien.Columns["HoTen"].HeaderText = "Họ và Tên";
            dgvSinhVien.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
            dgvSinhVien.Columns["GioiTinh"].HeaderText = "Giới Tính";
            dgvSinhVien.Columns["MaLop"].HeaderText = "Mã Lớp";

            dgvSinhVien.AllowUserToAddRows = false;

            cbLop.DataSource = ds.Tables["tblLop"].Copy();
            cbLop.DisplayMember = "TenLop";
            cbLop.ValueMember = "MaLop";
            cbLop.SelectedIndex = -1;

            this.chkNam.CheckedChanged += new System.EventHandler(this.chkGioiTinh_CheckedChanged);
            this.chkNu.CheckedChanged += new System.EventHandler(this.chkGioiTinh_CheckedChanged);

            KhoaSV(true);
        }
        private void chkGioiTinh_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                if (chk.Checked)
                {
                    if (chk.Name == "chkNam")
                    {
                        chkNu.Checked = false;
                    }
                    else if (chk.Name == "chkNu")
                    {
                        chkNam.Checked = false;
                    }
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            dangThemSV = true;

            txtMaSV.Clear();
            txtHoTen.Clear();

            dtpNgaySinh.Value = DateTime.Now;


            chkNam.Checked = true;
            chkNu.Checked = false;

            cbLop.SelectedIndex = -1;

            KhoaSV(false);
            txtMaSV.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!");
                return;
            }

            dangThemSV = false;
            KhoaSV(false);
            txtMaSV.ReadOnly = true;
            txtHoTen.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }


            string maSVCanXoa = dgvSinhVien.CurrentRow.Cells["MaSV"].Value.ToString();
            if (ds.Tables.Contains("tblDiem"))
            {
                DataRow[] diemRows = ds.Tables["tblDiem"].Select($"MaSV = '{maSVCanXoa}'");
                if (diemRows.Length > 0)
                {
                    MessageBox.Show("Không thể xóa sinh viên này vì đang có điểm số. Vui lòng xóa điểm trước!");
                    return;
                }
            }
            DialogResult traloi = MessageBox.Show(
               "Bạn có chắc muốn xóa sinh viên này?",
               "Xác nhận",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);

            if (traloi == DialogResult.No) return;

            try
            {
                int r = dgvSinhVien.CurrentCell.RowIndex;
                DataRow dr = ds.Tables["tblSV"].Rows[r];
                dr.Delete();

                daSV.Update(ds, "tblSV");
                //LoadDiemData();
                MessageBox.Show("Đã xóa thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message);
                ds.Tables["tblSV"].RejectChanges();
            }
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (txtMaSV.Text.Trim() == "" || txtHoTen.Text.Trim() == "" || cbLop.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã SV, Họ Tên và chọn Lớp!");
                return;
            }

            // Kiểm tra giới tính đã được chọn chưa
            if (!chkNam.Checked && !chkNu.Checked)
            {
                MessageBox.Show("Vui lòng chọn Giới tính!");
                return;
            }

            try
            {

                bool gioiTinhValue = chkNam.Checked;

                if (dangThemSV)
                {

                    DataRow[] timSV = ds.Tables["tblSV"].Select($"MaSV = '{txtMaSV.Text.Trim()}'");
                    if (timSV.Length > 0)
                    {
                        MessageBox.Show("Mã Sinh Viên đã tồn tại!");
                        txtMaSV.Focus();
                        return;
                    }

                    DataRow dong = ds.Tables["tblSV"].NewRow();

                    dong["MaSV"] = txtMaSV.Text.Trim();
                    dong["HoTen"] = txtHoTen.Text.Trim();
                    dong["NgaySinh"] = dtpNgaySinh.Value;
                    dong["GioiTinh"] = gioiTinhValue;
                    dong["MaLop"] = cbLop.SelectedValue;

                    ds.Tables["tblSV"].Rows.Add(dong);
                }


                else
                {

                    int r = dgvSinhVien.CurrentCell.RowIndex;


                    DataRow dr = ds.Tables["tblSV"].Rows[r];

                    dr["HoTen"] = txtHoTen.Text.Trim();
                    dr["NgaySinh"] = dtpNgaySinh.Value;
                    dr["GioiTinh"] = gioiTinhValue;
                    dr["MaLop"] = cbLop.SelectedValue;
                }

                daSV.Update(ds, "tblSV");

                KhoaSV(true); // 
                MessageBox.Show("Đã lưu thành công!");
            }


            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message);
                ds.Tables["tblSV"].RejectChanges();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            ds.Tables["tblSV"].RejectChanges();
            KhoaSV(true);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {

        }

        //=========================================Môn Học====================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA MÔN HỌC:
        private void KhoaMonHoc(bool daKhoa)
        {
            
            cbMaMon1.Enabled = !daKhoa || !dangThemMH;
            cbMonHoc1.Enabled = !daKhoa;
            cbSTC.Enabled = !daKhoa;

           
            btnLuu1.Enabled = !daKhoa;
            btnHuy1.Enabled = !daKhoa;

            btnThem1.Enabled = daKhoa;
            btnSua1.Enabled = daKhoa;
            btnXoa1.Enabled = daKhoa;
            btnTim1.Enabled = daKhoa;
        }
        private void LoadMonHoc()
        {
            string sqlMon = "SELECT MaMon, TenMon, SoTinChi FROM MonHoc";
            daMon = new SqlDataAdapter(sqlMon, conn);
            SqlCommandBuilder cbMonHoc1 = new SqlCommandBuilder(daMon);
            try
            {
                if (ds.Tables.Contains("tblMon"))
                    ds.Tables["tblMon"].Clear();
                daMon.Fill(ds, "tblMon"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu Môn Học: " + ex.Message);
            }

            
            dgvMonHoc.DataSource = ds.Tables["tblMon"];
            dgvMonHoc.AutoGenerateColumns = true;

           
            dgvMonHoc.Columns["MaMon"].HeaderText = "Mã Môn";
            dgvMonHoc.Columns["TenMon"].HeaderText = "Tên Môn";
            dgvMonHoc.Columns["SoTinChi"].HeaderText = "Số Tín Chỉ";

            dgvMonHoc.AllowUserToAddRows = false;

            KhoaMonHoc(true);

            
        }

        private void btnThem1_Click(object sender, EventArgs e)
        {
            dangThemMH = true;

            cbMaMon1.Text = "";
            cbMonHoc1.Text = "";
            cbSTC.Text = "";

            KhoaMonHoc(false);
            cbMaMon1.Focus();
        }

        private void btnSua1_Click(object sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!");
                return;
            }

            dangThemMH = false;
            KhoaMonHoc(false);
            cbMaMon1.Enabled = false; // Không cho sửa Mã Môn khi sửa
            cbMonHoc1.Focus();
        }

        private void btnXoa1_Click(object sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }

            string maMonCanXoa = dgvMonHoc.CurrentRow.Cells["MaMon"].Value.ToString();

            // Kiểm tra ràng buộc ngoại (giả sử bảng tblDiem đã được load)
            if (ds.Tables.Contains("tblDiem"))
            {
                DataRow[] diemRows = ds.Tables["tblDiem"].Select($"MaMon = '{maMonCanXoa}'");
                if (diemRows.Length > 0)
                {
                    MessageBox.Show("Không thể xóa môn học này vì đang có điểm số. Vui lòng xóa điểm trước!");
                    return;
                }
            }

            DialogResult traloi = MessageBox.Show("Bạn có chắc muốn xóa môn học này?","Xác nhận",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (traloi == DialogResult.No) return;
            try
            {
                int r = dgvMonHoc.CurrentCell.RowIndex;
                DataRow dr = ds.Tables["tblMon"].Rows[r];
                dr.Delete();

                daMon.Update(ds, "tblMon");
                MessageBox.Show("Đã xóa thành công!");
                LoadMonHoc(); // Tải lại môn học sau khi xóa
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message);
                ds.Tables["tblMon"].RejectChanges();
            }
        }

        private void btnLuu1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbMaMon1.Text) || string.IsNullOrWhiteSpace(cbMonHoc1.Text) || string.IsNullOrWhiteSpace(cbSTC.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã Môn, Tên Môn và Số Tín Chỉ!");
                return;
            }

            if (!int.TryParse(cbSTC.Text, out int soTinChi) || soTinChi <= 0)
            {
                MessageBox.Show("Số Tín Chỉ phải là số nguyên dương!");
                return;
            }

            try
            {
                if (dangThemMH)
                {
                    // Kiểm tra trùng MaMon
                    DataRow[] timMon = ds.Tables["tblMon"].Select($"MaMon = '{cbMaMon1.Text.Trim()}'");
                    if (timMon.Length > 0)
                    {
                        MessageBox.Show("Mã Môn đã tồn tại!");
                        cbMaMon1.Focus();
                        return;
                    }

                    // Thêm dòng mới
                    DataRow dong = ds.Tables["tblMon"].NewRow();
                    dong["MaMon"] = cbMaMon1.Text.Trim();
                    dong["TenMon"] = cbMonHoc1.Text.Trim();
                    dong["SoTinChi"] = soTinChi;

                    ds.Tables["tblMon"].Rows.Add(dong);
                }
                else // Chỉnh sửa
                {
                    int r = dgvMonHoc.CurrentCell.RowIndex;
                    DataRow dr = ds.Tables["tblMon"].Rows[r];

                    dr["TenMon"] = cbMonHoc1.Text.Trim();
                    dr["SoTinChi"] = soTinChi;
                }

                daMon.Update(ds, "tblMon");

                KhoaMonHoc(true);
                MessageBox.Show("Đã lưu thành công!");
                LoadMonHoc(); // Tải lại môn học sau khi lưu
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message);
                ds.Tables["tblMon"].RejectChanges();
            }
        }

        private void btnHuy1_Click(object sender, EventArgs e)
        {
            ds.Tables["tblMon"].RejectChanges();
            KhoaMonHoc(true);
        }

        private void btnThoat1_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim1_Click(object sender, EventArgs e)
        {
            string tuKhoa = cbMonHoc1.Text.Trim(); // Dùng cbMonHoc1 làm ô tìm kiếm tạm

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                dgvMonHoc.DataSource = ds.Tables["tblMon"]; // Hiển thị lại toàn bộ
                return;
            }

            DataTable dt = ds.Tables["tblMon"];

            string boLoc =
                $"MaMon LIKE '%{tuKhoa}%' OR TenMon LIKE '%{tuKhoa}%'";

            DataView dv = new DataView(dt);
            dv.RowFilter = boLoc;

            dgvMonHoc.DataSource = dv;
        }

        //=========================================Lớp Học====================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA LỚP HỌC:
        bool dangThem = false;
        private void KhoaLopHoc(bool daKhoa )
        {
            cbLop1.Enabled = !daKhoa;
            cbMonHoc2.Enabled = !daKhoa;
            txtSiSo.Enabled = !daKhoa;

            btnLuu2.Enabled = !daKhoa;
            btnHuy2.Enabled = !daKhoa;

            // Các nút Thêm, Sửa, Xóa chỉ bật khi khóa
            btnThem2.Enabled = daKhoa;
            btnSua2.Enabled = daKhoa;
            btnXoa2.Enabled = daKhoa;
        }
        private void LoadComboLop()
        {
            string sql = "SELECT * FROM Lop";
            daLop = new SqlDataAdapter(sql, conn);
            daLop.Fill(ds, "tblLop");

            cbLop1.DataSource = ds.Tables["tblLop"];
            cbLop1.DisplayMember = "TenLop";
            cbLop1.ValueMember = "MaLop";
        }
        private void LoadComboMon()
        {
            string sql = "SELECT * FROM MonHoc";
            daMon = new SqlDataAdapter(sql, conn);
            daMon.Fill(ds, "tblMon");

            cbMonHoc2.DataSource = ds.Tables["tblMon"];
            cbMonHoc2.DisplayMember = "TenMon";
            cbMonHoc2.ValueMember = "MaMon";
        }
        private void LoadDanhSachLop()
        {
            string sql = @"SELECT l.MaLop, l.TenLop, m.TenMon, 0 AS SiSo
                   FROM Lop l CROSS JOIN MonHoc m";

            daDSLop = new SqlDataAdapter(sql, conn);
            daDSLop.Fill(ds, "tblDSLop");

            dgvLopHoc.DataSource = ds.Tables["tblDSLop"];
            dgvLopHoc.Columns["MaLop"].HeaderText = "Mã Lớp";
            dgvLopHoc.Columns["TenLop"].HeaderText = "Tên Lớp";
            dgvLopHoc.Columns["TenMon"].HeaderText = "Môn Học";
            dgvLopHoc.Columns["SiSo"].HeaderText = "Sĩ Số";
        }
        private void btnThem2_Click(object sender, EventArgs e)
        {
            dangThem = true;

            cbLop1.SelectedIndex = -1;
            cbMonHoc2.SelectedIndex = -1;
            txtSiSo.Clear();

            KhoaLopHoc(false);
            cbLop1.Focus();
        }

        private void btnSua2_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!");
                return;
            }

            dangThem = false;
            KhoaLopHoc(false);
        }

        private void btnXoa2_Click(object sender, EventArgs e)
        {
            if (dgvLopHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }

            DialogResult traloi = MessageBox.Show(
                "Bạn có chắc muốn xóa lớp học này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (traloi == DialogResult.No) return;

            int r = dgvLopHoc.CurrentCell.RowIndex;
            ds.Tables["tblDSLop"].Rows[r].Delete();
        }

        private void btnLuu2_Click(object sender, EventArgs e)
        {
            if (cbLop1.SelectedIndex == -1 || cbMonHoc2.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Lớp và Môn học!");
                return;
            }

            if (dangThem)
            {
                // ===== THÊM MỚI =====
                DataRow dong = ds.Tables["tblDSLop"].NewRow();

                dong["MaLop"] = cbLop1.SelectedValue;
                dong["TenLop"] = cbLop1.Text;
                dong["TenMon"] = cbMonHoc2.Text;
                dong["SiSo"] = txtSiSo.Text;

                ds.Tables["tblDSLop"].Rows.Add(dong);
            }
            else
            {
                // ===== SỬA =====
                int r = dgvLopHoc.CurrentCell.RowIndex;

                ds.Tables["tblDSLop"].Rows[r]["MaLop"] = cbLop1.SelectedValue;
                ds.Tables["tblDSLop"].Rows[r]["TenLop"] = cbLop1.Text;
                ds.Tables["tblDSLop"].Rows[r]["TenMon"] = cbMonHoc2.Text;
                ds.Tables["tblDSLop"].Rows[r]["SiSo"] = txtSiSo.Text;
            }

            KhoaLopHoc(true);  // khóa lại sau khi lưu
        }

        private void btnHuy2_Click(object sender, EventArgs e)
        {
            KhoaLopHoc(true);
        }

        private void btnThoat2_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim2_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTim2.Text.Trim();
            if (tuKhoa == "")
            {
                MessageBox.Show("Vui lòng nhập từ khóa để tìm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable dt = ds.Tables["tblDSLop"];

            if (tuKhoa == "")
            {
                dgvLopHoc.DataSource = dt;
                return;
            }

            string boLoc =
                $"MaLop LIKE '%{tuKhoa}%' OR TenLop LIKE '%{tuKhoa}%'";

            DataView dv = new DataView(dt);
            dv.RowFilter = boLoc;

            dgvLopHoc.DataSource = dv;
        }
        //===========================================Nhập Điểm=================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA NHẬP ĐIỂM:
        bool dangThemDiem = false;
        SqlDataAdapter daDiem;

        private void LoadComboLop_NhapDiem()
        {
            string sql = "SELECT MaLop, TenLop FROM Lop";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cbLop2.DataSource = dt;
            cbLop2.DisplayMember = "TenLop"; 
            cbLop2.ValueMember = "MaLop"; 
            cbLop2.SelectedIndex = -1;
        }


        private void LoadComboMon_NhapDiem()
        {
            string sql = "SELECT MaMon, TenMon FROM MonHoc";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cbMaMon2.DataSource = dt;
            cbMaMon2.DisplayMember = "MaMon";
            cbMaMon2.ValueMember = "MaMon";

            cbMonHoc3.DataSource = dt.Copy();
            cbMonHoc3.DisplayMember = "TenMon";
            cbMonHoc3.ValueMember = "MaMon";

            // đồng bộ hai combobox
            cbMaMon2.SelectedIndexChanged += (s, e) => {
                if (cbMaMon2.SelectedValue != null)
                    cbMonHoc3.SelectedValue = cbMaMon2.SelectedValue;
            };
            cbMonHoc3.SelectedIndexChanged += (s, e) => {
                if (cbMonHoc3.SelectedValue != null)
                    cbMaMon2.SelectedValue = cbMonHoc3.SelectedValue;
            };
        }
        private void LoadNhapDiem()
        {
            daMon = new SqlDataAdapter("SELECT MaMon, TenMon FROM MonHoc", conn);

            if (ds.Tables["tblMonHoc"] != null)
                ds.Tables["tblMonHoc"].Clear();

            daMon.Fill(ds, "tblMonHoc");

            cbMaMon2.DataSource = ds.Tables["tblMonHoc"];
            cbMaMon2.DisplayMember = "TenMon";
            cbMaMon2.ValueMember = "MaMon";


            string sql = @"
        SELECT d.MaSV, sv.HoTen, sv.MaLop, d.MaMon, mh.TenMon,
               d.DiemGK, d.DiemCK,
               ROUND((d.DiemGK + d.DiemCK * 2) / 3, 2) AS DiemTB
        FROM Diem d
        JOIN SinhVien sv ON d.MaSV = sv.MaSV
        JOIN MonHoc mh ON d.MaMon = mh.MaMon";

            daDSLop = new SqlDataAdapter(sql, conn);

            if (ds.Tables["tblDiem"] != null)
                ds.Tables["tblDiem"].Clear();

            daDSLop.Fill(ds, "tblDiem");

            dgvNhapDiem.DataSource = ds.Tables["tblDiem"];
        }

        private void LoadBangDiem()
        {
            string sql = @"SELECT d.MaSV, sv.HoTen, sv.MaLop, d.MaMon, m.TenMon,
                          d.DiemGK, d.DiemCK, d.DiemTB
                   FROM Diem d
                   JOIN SinhVien sv ON d.MaSV = sv.MaSV
                   JOIN MonHoc m ON d.MaMon = m.MaMon";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.Columns.Add("KetQua", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                double tb = Convert.ToDouble(row["DiemTB"]);
                row["KetQua"] = (tb >= 5) ? "Đậu" : "Rớt";
            }


            dgvNhapDiem.DataSource = dt;
            dgvNhapDiem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNhapDiem.Columns["HoTen"].FillWeight = 200;
            dgvNhapDiem.Columns["TenMon"].FillWeight = 150;
            dgvNhapDiem.AllowUserToAddRows = false;

            // đặt header tiếng Việt (nếu cột tồn tại)
            if (dgvNhapDiem.Columns.Contains("MaSV")) dgvNhapDiem.Columns["MaSV"].HeaderText = "Mã SV";
            if (dgvNhapDiem.Columns.Contains("HoTen")) dgvNhapDiem.Columns["HoTen"].HeaderText = "Họ Tên";
            if (dgvNhapDiem.Columns.Contains("MaLop")) dgvNhapDiem.Columns["MaLop"].HeaderText = "Lớp";
            if (dgvNhapDiem.Columns.Contains("MaMon")) dgvNhapDiem.Columns["MaMon"].HeaderText = "Mã Môn";
            if (dgvNhapDiem.Columns.Contains("TenMon")) dgvNhapDiem.Columns["TenMon"].HeaderText = "Tên Môn";
            if (dgvNhapDiem.Columns.Contains("DiemGK")) dgvNhapDiem.Columns["DiemGK"].HeaderText = "Giữa kỳ";
            if (dgvNhapDiem.Columns.Contains("DiemCK")) dgvNhapDiem.Columns["DiemCK"].HeaderText = "Cuối kỳ";
            if (dgvNhapDiem.Columns.Contains("DiemTB")) dgvNhapDiem.Columns["DiemTB"].HeaderText = "Trung bình";
            if (dgvNhapDiem.Columns.Contains("KetQua")) dgvNhapDiem.Columns["KetQua"].HeaderText = "Kết quả";
        }
        private void cbLop2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLop2.SelectedValue == null) return;
            string malop = cbLop2.SelectedValue.ToString();

            string sql = @"SELECT d.MaSV, sv.HoTen, sv.MaLop, d.MaMon, m.TenMon,
                          d.DiemGK, d.DiemCK, d.DiemTB
                   FROM Diem d
                   JOIN SinhVien sv ON d.MaSV = sv.MaSV
                   JOIN MonHoc m ON d.MaMon = m.MaMon
                   WHERE sv.MaLop = @MaLop";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@MaLop", malop);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvNhapDiem.DataSource = dt;
        }
        private void KhoaNhapDiem(bool daKhoa)
        {
            cbLop2.Enabled = !daKhoa;
            cbMaMon2.Enabled = !daKhoa;
            cbMonHoc3.Enabled = !daKhoa;

            txtMaSV1.Enabled = !daKhoa;
            txtHoTen1.Enabled = false;
            txtDiemGK1.Enabled = !daKhoa;
            txtDiemCK1.Enabled = !daKhoa;

            btnLuu3.Enabled = !daKhoa;
            btnHuy3.Enabled = !daKhoa;

            btnThem3.Enabled = daKhoa;
            btnSua3.Enabled = daKhoa;
            btnXoa3.Enabled = daKhoa;
        }
        private void XoaNhapDiem()
        {
            txtMaSV1.Text = "";
            txtHoTen1.Text = "";
            txtDiemGK1.Text = "";
            txtDiemCK1.Text = "";
            txtDiemTB1.Text = "";
            txtKetQua1.Text = "";
        }
        private void dgvNhapDiem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvNhapDiem.Rows[e.RowIndex];

            txtMaSV1.Text = row.Cells["MaSV"].Value?.ToString() ?? "";
            txtHoTen1.Text = row.Cells["HoTen"].Value?.ToString() ?? "";

            string maMon = row.Cells["MaMon"].Value?.ToString() ?? "";
            if (!string.IsNullOrEmpty(maMon))
            {
                cbMaMon2.SelectedValue = maMon;
                cbMonHoc3.SelectedValue = maMon;
            }

            txtDiemGK1.Text = row.Cells["DiemGK"].Value?.ToString() ?? "";
            txtDiemCK1.Text = row.Cells["DiemCK"].Value?.ToString() ?? "";
            txtDiemTB1.Text = row.Cells["DiemTB"].Value?.ToString() ?? "";
            txtKetQua1.Text = row.Cells["KetQua"].Value?.ToString() ?? "";
        }
        private void btnThem3_Click(object sender, EventArgs e)
        {
            dangThemDiem = true;
            XoaNhapDiem();
            KhoaNhapDiem(false);
            LoadNhapDiem();
            txtMaSV1.Focus();
        }

        private void btnSua3_Click(object sender, EventArgs e)
        {
            if (dgvNhapDiem.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dangThemDiem = false;

            KhoaNhapDiem(false);

            // Không cho sửa mã sinh viên và họ tên
            txtMaSV1.Enabled = false;
            txtHoTen1.Enabled = false;

            // Cho sửa điểm và môn học
            txtDiemGK1.Enabled = true;
            txtDiemCK1.Enabled = true;
            cbMaMon2.Enabled = true;
            cbMonHoc3.Enabled = true;
            LoadNhapDiem();
        }

        private void btnXoa3_Click(object sender, EventArgs e)
        {
            string maSV = txtMaSV1.Text;
            string maMon = cbMaMon2.Text;

            DialogResult d = MessageBox.Show(
                $"Bạn có chắc muốn xóa điểm?\n\nMã SV: {maSV}\nMã Môn: {maMon}",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (d == DialogResult.No) return;

            string query = "DELETE FROM Diem WHERE MaSV = @maSV AND MaMon = @maMon";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@maSV", maSV);
                    cmd.Parameters.AddWithValue("@maMon", maMon);

                    int kq = cmd.ExecuteNonQuery();

                    if (kq > 0)
                    {
                        MessageBox.Show("Xóa điểm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadBangDiem();
                        XoaNhapDiem();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu để xóa!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            LoadNhapDiem();
        }

        private void btnLuu3_Click(object sender, EventArgs e)
        {
            // ===== KIỂM TRA ĐẦU VÀO =====
            if (cbLop2.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtMaSV1.Text) ||
                cbMaMon2.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Lớp - Sinh viên - Môn học!");
                return;
            }

            // kiểm tra điểm nhập có hợp lệ không
            if (!double.TryParse(txtDiemGK1.Text, out double gk) ||
                !double.TryParse(txtDiemCK1.Text, out double ck))
            {
                MessageBox.Show("Điểm phải là số!");
                return;
            }

            if (gk < 0 || gk > 10 || ck < 0 || ck > 10)
            {
                MessageBox.Show("Điểm phải nằm trong khoảng 0 - 10!");
                return;
            }

            // ===== TÍNH ĐIỂM TB =====
            double tb = Math.Round((gk + ck * 2) / 3, 2);
            string ketQua = tb >= 5 ? "Đậu" : "Rớt";

            if (dangThem)
            {
                // ===== THÊM MỚI =====
                DataRow dong = ds.Tables["tblDiem"].NewRow();

                dong["MaSV"] = txtMaSV1.Text;
                dong["MaMon"] = cbMaMon2.SelectedValue;
                dong["DiemGK"] = gk;
                dong["DiemCK"] = ck;
                dong["DiemTB"] = tb;
                dong["KetQua"] = ketQua;

                ds.Tables["tblDiem"].Rows.Add(dong);
            }
            else
            {
                // ===== SỬA =====
                int r = dgvNhapDiem.CurrentCell.RowIndex;

                ds.Tables["tblDiem"].Rows[r]["MaSV"] = txtMaSV1.Text;
                ds.Tables["tblDiem"].Rows[r]["MaMon"] = cbMaMon2.SelectedValue;
                ds.Tables["tblDiem"].Rows[r]["DiemGK"] = gk;
                ds.Tables["tblDiem"].Rows[r]["DiemCK"] = ck;
                ds.Tables["tblDiem"].Rows[r]["DiemTB"] = tb;
                ds.Tables["tblDiem"].Rows[r]["KetQua"] = ketQua;
            }

            // ===== KHÓA LẠI GIAO DIỆN =====
            KhoaNhapDiem(true);

            // ===== LOAD LẠI BẢNG =====
            LoadBangDiem();
        }

        private void btnHuy3_Click(object sender, EventArgs e)
        {
            KhoaNhapDiem(true);
        }

        private void btnThoat3_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?", "Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnTim3_Click(object sender, EventArgs e)
        {

        }
        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            LoadNhapDiem();
        }
        //============================================Tra Cứu================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA TRA CỨU:
        private void LoadTraCuuData()
        {
            // Truy vấn JOIN 4 bảng: SinhVien, Lop, MonHoc, Diem (Bổ sung SoTinChi)
            string sqlTraCuu = @"
                SELECT 
                    SV.MaSV, SV.HoTen, L.TenLop, SV.NgaySinh, SV.GioiTinh,
                    MH.MaMon, MH.TenMon, MH.SoTinChi,
                    D.DiemGK, D.DiemCK, D.DiemTB
                FROM Diem D
                JOIN SinhVien SV ON D.MaSV = SV.MaSV
                JOIN MonHoc MH ON D.MaMon = MH.MaMon
                JOIN Lop L ON SV.MaLop = L.MaLop";

            if (ds.Tables.Contains("tblTraCuu"))
                ds.Tables["tblTraCuu"].Clear();

            daTraCuu = new SqlDataAdapter(sqlTraCuu, conn);
            daTraCuu.Fill(ds, "tblTraCuu");
        }

        private void btnTim4_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTim4.Text.Trim();

            LoadTraCuuData(); // Tải lại data gốc

            DataTable dt = ds.Tables["tblTraCuu"];

            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                dgvTraCuu.DataSource = dt; // Hiển thị lại toàn bộ
                return;
            }

            // Lọc trên các cột quan trọng
            string boLoc =
                $"MaSV LIKE '%{tuKhoa}%' OR HoTen LIKE '%{tuKhoa}%' OR TenLop LIKE '%{tuKhoa}%' OR TenMon LIKE '%{tuKhoa}%'";

            DataView dv = new DataView(dt);
            dv.RowFilter = boLoc;

            dgvTraCuu.DataSource = dv;
        }

        private void tabQLSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabQLSinhVien.SelectedTab == tabLopHoc)
            {
                KhoaLopHoc(true);
            }
        }

        private void tabSinhVien_Click(object sender, EventArgs e)
        {

        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (!btnThem.Enabled || !btnSua.Enabled) return;

            if (e.RowIndex < 0 || e.RowIndex >= dgvSinhVien.RowCount) return; 

            DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];

            
            txtMaSV.Text = row.Cells["MaSV"].Value.ToString();
            txtHoTen.Text = row.Cells["HoTen"].Value.ToString();

            
            if (row.Cells["NgaySinh"].Value != DBNull.Value && row.Cells["NgaySinh"].Value != null)
            {
                
                dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["NgaySinh"].Value);
            }
            else
            {
                dtpNgaySinh.Value = DateTime.Now; 
            }

            
            if (row.Cells["GioiTinh"].Value != DBNull.Value && row.Cells["GioiTinh"].Value != null)
            {
                bool gioiTinhNam = Convert.ToBoolean(row.Cells["GioiTinh"].Value); // 1: Nam, 0: Nữ
                chkNam.Checked = gioiTinhNam;
                chkNu.Checked = !gioiTinhNam;
            }
            else
            {
                chkNam.Checked = true; 
                chkNu.Checked = false;
            }

            
            if (row.Cells["MaLop"].Value != DBNull.Value && row.Cells["MaLop"].Value != null)
            {
                cbLop.SelectedValue = row.Cells["MaLop"].Value;
            }
            else
            {
                cbLop.SelectedIndex = -1;
            }
        }
    }
 }

