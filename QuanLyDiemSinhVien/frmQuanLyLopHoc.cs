using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyDiemSinhVien
{
    public partial class frmQuanLyLopHoc : Form
    {
        string flag = "";
        public frmQuanLyLopHoc()
        {
            InitializeComponent();
        }

        private void frmQuanLyLopHoc_Load(object sender, EventArgs e)
        {

        }
        private void KhoaNhap(bool enable)
        {
            cbLop.Enabled = enable;
            cbMonHoc.Enabled = enable;
            txtSiSo.Enabled = enable;

            btnLuu.Enabled = enable;
            btnHuy.Enabled = enable;

            btnThem.Enabled = !enable;
            btnSua.Enabled = !enable;
            btnXoa.Enabled = !enable;
        }

        private void TaiDanhSach()
        {
            string query = @"
        SELECT Lop, TenMon, SiSo
        FROM LopHoc";

            //dataGridView1.DataSource = Database.LoadData(query);
        }

        private void LoadComboLop()
        {
            //cbLop.DataSource = Database.LoadData("SELECT MaLop FROM Lop");
            cbLop.DisplayMember = "MaLop";
            cbLop.ValueMember = "MaLop";
        }


        private void LoadComboMonHoc()
        {
            //cbMonHoc.DataSource = Database.LoadData("SELECT MaMon, TenMon FROM MonHoc");
            cbMonHoc.DisplayMember = "TenMon";
            cbMonHoc.ValueMember = "MaMon";
        }
        private bool KiemTraNhapDayDu()
        {
            if (cbLop.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn lớp!");
                return false;
            }

            if (cbMonHoc.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn môn!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSiSo.Text))
            {
                MessageBox.Show("Vui lòng nhập sĩ số!");
                return false;
            }

            int so;
            if (!int.TryParse(txtSiSo.Text, out so))
            {
                MessageBox.Show("Sĩ số phải là số!");
                return false;
            }

            return true;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc muốn thoát chương trình không?","Thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            KhoaNhap(false);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string lop = cbLop.Text;
            string mon = cbMonHoc.Text;

            string query = $"DELETE FROM LopHoc WHERE Lop = '{lop}' AND TenMon = '{mon}'";
            //Database.Execute(query);

            TaiDanhSach();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            flag = "add";
            KhoaNhap(true);

            cbLop.SelectedIndex = -1;
            cbMonHoc.SelectedIndex = -1;
            txtSiSo.Clear();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {

        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!KiemTraNhapDayDu())
                return;
        }
    }
}
