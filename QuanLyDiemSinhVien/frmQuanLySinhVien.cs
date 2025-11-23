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
        public frmQuanLySinhVien()
        {
            InitializeComponent();
        }
        DataSet ds = new DataSet("dsQLNV");
        SqlDataAdapter daLop;
        SqlDataAdapter daMon;
        SqlDataAdapter daDSLop;
        //....
        private void frmQuanLySinhVien_Load(object sender, EventArgs e)
        {
            //=========================Xử lý load Sinh Viên=========================================
            //...
            //=========================Xử lý load Môn Học===========================================
            //...
            //=========================Xử lý load Lớp Học===========================================
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=.;Initial Catalog=QuanlyDSV;Integrated Security=True";
            // ==== Load bảng Lớp ====
            string sqlLop = "SELECT * FROM Lop";
            daLop = new SqlDataAdapter(sqlLop, conn);
            daLop.Fill(ds, "tblLop");

            cbLop1.DataSource = ds.Tables["tblLop"];
            cbLop1.DisplayMember = "TenLop";
            cbLop1.ValueMember = "MaLop";

            // ==== Load bảng Môn học ====
            string sqlMon = "SELECT * FROM MonHoc";
            daMon = new SqlDataAdapter(sqlMon, conn);
            daMon.Fill(ds, "tblMon");

            cbMonHoc2.DataSource = ds.Tables["tblMon"];
            cbMonHoc2.DisplayMember = "TenMon";
            cbMonHoc2.ValueMember = "MaMon";

            // ==== Load danh sách lớp học (grid) ====
            string sqlDSLop = @"SELECT l.MaLop, l.TenLop, m.TenMon, 0 AS SiSo
                        FROM Lop l
                        CROSS JOIN MonHoc m";

            daDSLop = new SqlDataAdapter(sqlDSLop, conn);
            daDSLop.Fill(ds, "tblDSLop");

            dgvLopHoc.AutoGenerateColumns = true;

            dgvLopHoc.DataSource = ds.Tables["tblDSLop"]; 

            dgvLopHoc.Columns["MaLop"].HeaderText = "Lớp";
            dgvLopHoc.Columns["TenLop"].HeaderText = "Tên Lớp";
            dgvLopHoc.Columns["TenMon"].HeaderText = "Môn Học";
            dgvLopHoc.Columns["SiSo"].HeaderText = "Sĩ Số";

            dgvLopHoc.AllowUserToAddRows = false;


            KhoaLopHoc(true);
            //=========================Xử lý load Nhập Điểm=========================================
            //...
        }

        //==========================================Sinh Viên================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA SINH VIÊN:
        //..................................
        private void btnThem_Click(object sender, EventArgs e)
        {

        }

        private void btnSua_Click(object sender, EventArgs e)
        {

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {

        }

        private void btnLuu_Click(object sender, EventArgs e)
        {

        }

        private void btnHuy_Click(object sender, EventArgs e)
        {

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
        //..................................
        private void btnThem1_Click(object sender, EventArgs e)
        {

        }

        private void btnSua1_Click(object sender, EventArgs e)
        {

        }

        private void btnXoa1_Click(object sender, EventArgs e)
        {

        }

        private void btnLuu1_Click(object sender, EventArgs e)
        {

        }

        private void btnHuy1_Click(object sender, EventArgs e)
        {

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
        //..................................
        private void btnThem3_Click(object sender, EventArgs e)
        {

        }

        private void btnSua3_Click(object sender, EventArgs e)
        {

        }

        private void btnXoa3_Click(object sender, EventArgs e)
        {

        }

        private void btnLuu3_Click(object sender, EventArgs e)
        {

        }

        private void btnHuy3_Click(object sender, EventArgs e)
        {

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

        //============================================Tra Cứu================================================
        //CHỖ VIẾT THÊM HÀM MỚI CỦA TRA CỨU:
        //..................................
        private void btnTim4_Click(object sender, EventArgs e)
        {

        }

        private void tabQLSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabQLSinhVien.SelectedTab == tabLopHoc)
            {
                KhoaLopHoc(true);
            }
        }
    }
}
