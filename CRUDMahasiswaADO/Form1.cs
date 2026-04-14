using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class FormMahasiswa : Form
    {
        private readonly string connectionString = "Data Source=Faizsltn-27\\FAIZSLTN; Initial Catalog=DBAkademikADO; Integrated Security=True";
        private SqlConnection conn;

        public FormMahasiswa()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbJK.Items.Clear();
            cmbJK.Items.Add("L");
            cmbJK.Items.Add("P");

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                MessageBox.Show("Koneksi berhasil");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectDatabase();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("NIM", "NIM");
                dataGridView1.Columns.Add("Nama", "Nama");
                dataGridView1.Columns.Add("JenisKelamin", "Jenis Kelamin");
                dataGridView1.Columns.Add("TanggalLahir", "Tanggal Lahir");
                dataGridView1.Columns.Add("Alamat", "Alamat");
                dataGridView1.Columns.Add("KodeProdi", "Kode Prodi");

                string query = "SELECT * FROM Mahasiswa";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["NIM"].ToString(),
                        reader["Nama"].ToString(),
                        reader["JenisKelamin"].ToString(),
                        Convert.ToDateTime(reader["TanggalLahir"]).ToShortDateString(),
                        reader["Alamat"].ToString(),
                        reader["KodeProdi"].ToString()
                    );
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan data: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                if (string.IsNullOrEmpty(textNIM.Text)) { MessageBox.Show("NIM harus diisi"); return; }

                string query = "INSERT INTO Mahasiswa (NIM, Nama, JenisKelamin, TanggalLahir, Alamat, KodeProdi) " +
                               "VALUES (@NIM, @Nama, @JK, @TanggalLahir, @Alamat, @KodeProdi)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", textNIM.Text);
                cmd.Parameters.AddWithValue("@Nama", textNama.Text);
                cmd.Parameters.AddWithValue("@JK", cmbJK.Text);
                cmd.Parameters.AddWithValue("@TanggalLahir", dptTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@Alamat", textAlamat.Text);
                cmd.Parameters.AddWithValue("@KodeProdi", textKodeProdi.Text);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Data mahasiswa berhasil ditambahkan");
                    ClearForm();
                    btnLoad.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string query = "UPDATE Mahasiswa SET Nama=@Nama, JenisKelamin=@JK, TanggalLahir=@TanggalLahir, " +
                               "Alamat=@Alamat, KodeProdi=@KodeProdi WHERE NIM=@NIM";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", textNIM.Text);
                cmd.Parameters.AddWithValue("@Nama", textNama.Text);
                cmd.Parameters.AddWithValue("@JK", cmbJK.Text);
                cmd.Parameters.AddWithValue("@TanggalLahir", dptTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@Alamat", textAlamat.Text);
                cmd.Parameters.AddWithValue("@KodeProdi", textKodeProdi.Text);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Data berhasil diupdate");
                    btnLoad.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                DialogResult confirm = MessageBox.Show("Yakin ingin menghapus data?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    string query = "DELETE FROM Mahasiswa WHERE NIM = @NIM";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NIM", textNIM.Text);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Data berhasil dihapus");
                        ClearForm();
                        btnLoad.PerformClick();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            textNIM.Clear();
            textNama.Clear();
            cmbJK.SelectedIndex = -1;
            textAlamat.Clear();
            textKodeProdi.Clear();
            dptTanggalLahir.Value = DateTime.Now;
            textNIM.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Memindahkan data dari tabel ke form pengisian
                textNIM.Text = row.Cells["NIM"].Value.ToString();
                textNama.Text = row.Cells["Nama"].Value.ToString();
                cmbJK.Text = row.Cells["JenisKelamin"].Value.ToString();
                dptTanggalLahir.Value = Convert.ToDateTime(row.Cells["TanggalLahir"].Value);
                textAlamat.Text = row.Cells["Alamat"].Value.ToString();
                textKodeProdi.Text = row.Cells["KodeProdi"].Value.ToString();
            }
        }
    }
}