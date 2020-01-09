using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

namespace imagesearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //veritabanı bağlantısı burada yer almaktadır
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-0OLBCM7\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=DbNotKayit");

        string resimpath;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title="yüklenicek fotografı seçiniz...";
            openFileDialog1.Filter = "Jpeg Dosyası (*.jpg)|.jpg|Png Dosyası (*.png)|.png";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.CheckFileExists = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                resimpath = openFileDialog1.FileName.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //resim yüklemek için filestream metodunu kullanıyoruz...
            FileStream fs = new FileStream(resimpath,FileMode.Open,FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] resim = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            //veritabanı
            SqlCommand komut = new SqlCommand("insert into image(imagedata) values (@p1)",baglanti);
            komut.Parameters.Add("@p1",SqlDbType.Image, resim.Length).Value = resim;
            //hata almamak için try kullanıcağız
            try
            {
                baglanti.Open();
                komut.ExecuteNonQuery();
                MessageBox.Show("veritabanına başarıyla kayıt edilmiştir");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {

                baglanti.Close();
            }
            this.imageTableAdapter.Fill(this.dataSet1.image);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet1.image' table. You can move, or remove it, as needed.
            this.imageTableAdapter.Fill(this.dataSet1.image);

        }
        string secilen2;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            secilen2 = dataGridView1.Rows[secilen].Cells[0].Value.ToString();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select * from image where ID='" + secilen2 + "'",baglanti);
            SqlDataReader dr = komut.ExecuteReader();
            if (dr.Read())
            {
                //binary olarak eklediğimiz image geri çevirerek dönüştürüp picturebox2'ye bastırdık
                if (dr["imagedata"] != null)
                {
                    Byte[] data = new Byte[0];
                    data = (Byte[])(dr["imagedata"]);
                    MemoryStream mem = new MemoryStream(data);
                    pictureBox2.Image = Image.FromStream(mem);
                }
                dr.Close();
                komut.Dispose();
                baglanti.Close();
            }
            this.imageTableAdapter.Fill(this.dataSet1.image);
        }

    }
}
