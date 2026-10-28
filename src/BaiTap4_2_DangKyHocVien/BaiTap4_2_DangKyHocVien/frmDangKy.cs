using Microsoft.Data.SqlClient; // Thư viện kết nối SQL Server
using System;
using System.Data;
using System.Windows.Forms;

namespace BaiTap4_2_DangKyHocVien
{
    public partial class frmDangKy : Form
    {
        public frmDangKy()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        // Hàm tải dữ liệu lên lưới
        private void TaiDuLieu()
        {
            // Dùng lại chuỗi kết nối của bạn
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=ql_sinhvien;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM HocVien"; // Lấy toàn bộ học viên
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt); // Đổ dữ liệu vào bảng tạm

                dgvHocVien.DataSource = dt; // Hiển thị lên DataGridView
            }
        }

        private void frmDangKy_Load(object sender, EventArgs e)
        {
            // Tạo danh sách Khóa học giả lập
            var danhSachKhoaHoc = new[]
            {
                new { MaKhoa = "CNTT", TenKhoa = "Công nghệ thông tin" },
                new { MaKhoa = "KT", TenKhoa = "Kế toán" },
                new { MaKhoa = "QTKD", TenKhoa = "Quản trị kinh doanh" },
                new { MaKhoa = "NN", TenKhoa = "Ngôn ngữ Anh" }
            };

            // Nạp dữ liệu vào ComboBox
            cboCourse.DataSource = danhSachKhoaHoc;
            cboCourse.DisplayMember = "TenKhoa"; // Hiển thị tên khóa học
            cboCourse.ValueMember = "MaKhoa";    // Giá trị ngầm là mã khóa học

            TaiDuLieu(); // Gọi hàm tải dữ liệu
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            // 1. Lấy thông tin cơ bản
            string hoTen = txtHoTen.Text.Trim();
            string sdt = mtxtPhone.Text;
            DateTime ngaySinh = dtpBirthDate.Value;
            string khoaHoc = cboCourse.Text;

            // 2. Xử lý Giới tính
            string gioiTinh = "Chưa chọn";
            if (rdoNam.Checked) gioiTinh = "Nam";
            else if (rdoNu.Checked) gioiTinh = "Nữ";

            // 3. Xử lý Sở thích
            string soThich = "";
            if (chkDocSach.Checked) soThich += "Đọc sách, ";
            if (chkNgheNhac.Checked) soThich += "Nghe nhạc, ";
            if (chkTheThao.Checked) soThich += "Thể thao, ";
            if (soThich.EndsWith(", ")) soThich = soThich.Substring(0, soThich.Length - 2);

            // 4. Kiểm tra dữ liệu rỗng (Đã nâng cấp thêm điều kiện MaskCompleted)
            if (string.IsNullOrWhiteSpace(hoTen) || !mtxtPhone.MaskCompleted)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Họ tên và Số điện thoại (đủ 10 số)!", "Cảnh báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. LƯU VÀO CƠ SỞ DỮ LIỆU
            // Chuỗi kết nối tới file CSDL nằm trong thư mục dự án
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=ql_sinhvien;Trusted_Connection=True;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Câu lệnh INSERT
                    string query = @"INSERT INTO HocVien (HoTen, SDT, NgaySinh, KhoaHoc, GioiTinh, SoThich) 
                                     VALUES (@HoTen, @SDT, @NgaySinh, @KhoaHoc, @GioiTinh, @SoThich)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Truyền tham số
                        cmd.Parameters.AddWithValue("@HoTen", hoTen);
                        cmd.Parameters.AddWithValue("@SDT", sdt);
                        cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                        cmd.Parameters.AddWithValue("@KhoaHoc", khoaHoc);
                        cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                        cmd.Parameters.AddWithValue("@SoThich", soThich);

                        // Thực thi lệnh
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Đăng ký học viên thành công và đã lưu vào CSDL!", "Thông báo",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // (Tùy chọn) Xóa trắng form sau khi đăng ký thành công để nhập người tiếp theo
                        // txtHoTen.Clear();
                        // mtxtPhone.Clear();
                        // rdoNam.Checked = false;
                        // rdoNu.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TaiDuLieu(); // Cập nhật lại lưới ngay lập tức
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã chọn dòng nào trên lưới chưa
            if (dgvHocVien.CurrentRow == null || dgvHocVien.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Vui lòng chọn một học viên trên bảng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy Mã Học Viên (MaHV) của dòng đang được chọn
            // (Lưu ý: Cột này phải có tên chính xác là "MaHV" giống trong SQL)
            string maHV = dgvHocVien.CurrentRow.Cells["MaHV"].Value.ToString();
            string tenHV = dgvHocVien.CurrentRow.Cells["HoTen"].Value.ToString();

            // 3. Hỏi lại cho chắc chắn trước khi xóa
            DialogResult xacNhan = MessageBox.Show($"Bạn có chắc chắn muốn xóa học viên '{tenHV}' không?",
                                                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (xacNhan == DialogResult.Yes)
            {
                string connectionString = @"Server=localhost\SQLEXPRESS;Database=ql_sinhvien;Trusted_Connection=True;TrustServerCertificate=True;";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        // Câu lệnh DELETE
                        string query = "DELETE FROM HocVien WHERE MaHV = @MaHV";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaHV", maHV);
                            cmd.ExecuteNonQuery(); // Thực thi lệnh xóa
                        }
                    }

                    MessageBox.Show("Đã xóa học viên thành công!", "Thông báo");

                    // 4. Gọi lại hàm tải dữ liệu để bảng trên màn hình tự cập nhật (mất dòng vừa xóa)
                    TaiDuLieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}