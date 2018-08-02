using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zsbApp.SQLServerBackup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = this.getConnectionString();
            if (string.IsNullOrEmpty(connectionString))
                return;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = System.DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + ".bak";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string fileName = dialog.FileName;
            //if (File.Exists(fileName))
            //{
            //    MessageBox.Show("信息未完整。");
            //    return;
            //}
            this.button1.Enabled = false;

            string sql = string.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", this.txbDatabase.Text.Trim(), fileName);

            System.Timers.Timer t = new System.Timers.Timer(50);
            t.Enabled = true;
            t.Elapsed += (a, b) =>
            {
                t.Enabled = false;
                try
                {
                    SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql, conn);
                    comm.CommandType = CommandType.Text;
                    comm.ExecuteNonQuery();
                    conn.Close();
                    this.setEnabled();
                    MessageBox.Show("ok");
                }
                catch (Exception ex)
                {
                    this.setEnabled();
                    MessageBox.Show(ex.Message);
                }
            };
        }

        public string getConnectionString()
        {
            if (string.IsNullOrEmpty(txbServer.Text.Trim()) || string.IsNullOrEmpty(txbDatabase.Text.Trim()) || string.IsNullOrEmpty(txbUsername.Text.Trim()))
            {
                MessageBox.Show("信息未完整。");
                return null;
            }
            return string.Format("server={0};database={1};uid={2};pwd={3}", txbServer.Text.Trim(), txbDatabase.Text.Trim(), txbUsername.Text.Trim(), txbPassword.Text);
        }

        private void setEnabled()
        {
            this.Invoke(new Action(() => { this.button1.Enabled = true; }));
        }
    }
}
