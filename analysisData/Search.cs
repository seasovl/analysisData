using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace analysisData
{
    public partial class Search : Form
    {
        public Search()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();
            if(dr == DialogResult.OK){
                string fileName = ofd.FileName;
                DataImport import = new DataImport();
                import.FileName = fileName;
                DialogResult idr = import.ShowDialog();
                Search_Load(sender, e);
            }
        }

        private void Search_Load(object sender, EventArgs e)
        {
            DirectoryInfo dir=new DirectoryInfo("databases");
            if (!dir.Exists)//如果文件不存在
            {
                dir.Create();
            }
            database = dir.GetFiles();
            this.comboBox1.DataSource = database;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = comboBox4.Items.Count-1;
            this.comboBox3.SelectedIndex = 0;
        }

        private FileInfo[] database;
        private SqliteDBHelper sqlitedb=new SqliteDBHelper();
        private string[] datafm = {"000000","00000","0000","000","00","0",""};
        private void button2_Click(object sender, EventArgs e)
        {
            //条件 
            //1 获得数据库
            string dbName = this.comboBox1.Text;
            if (dbName.Trim().Equals("")) {
                MessageBox.Show("请导入数据!!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dbName = dbName.Substring(0, dbName.IndexOf(".db"));
            //2 开始,结束日期
            DateTime beginDate = this.dateTimePicker1.Value;
            DateTime endDate = this.dateTimePicker2.Value;
            string begd = beginDate.ToString("yyyyMMdd");
            string endd = endDate.ToString("yyyyMMdd");
            //3 开始,结束时间段
            string begt = comboBox3.Text;
            string endt = comboBox4.Text;
            //4 统计维度
            // 0 分 1 时 2 天 3 周
           int selIndex = this.comboBox2.SelectedIndex;


            SQLiteConnection conn = sqlitedb.getSqlLiteDB(dbName);
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            List<AnalyBean> lists = sqlitedb.queryAnaly(cmd, begd, endd, begt + "0000", endt + "0000", selIndex);
                this.listView1.Items.Clear();
            foreach(AnalyBean ab in lists){
                ListViewItem lvi = new ListViewItem(ab.Datea.ToString());//创建列表项
                DateTime time = DateTime.ParseExact(datafm[ab.Timea.ToString().Length] + ab.Timea.ToString(), "HHmmss", null);
                lvi.SubItems.Add(time.ToShortTimeString());
                lvi.SubItems.Add(ab.Higha.ToString());
                lvi.SubItems.Add(ab.Lowa.ToString());
                this.listView1.Items.Add(lvi);
            }
            conn.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (this.dateTimePicker1.Value > this.dateTimePicker2.Value)
            {
                MessageBox.Show("日期选择不正确!!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.dateTimePicker1.Value = this.dateTimePicker2.Value;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (this.dateTimePicker1.Value > this.dateTimePicker2.Value)
            {
                MessageBox.Show("日期选择不正确!!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.dateTimePicker2.Value = this.dateTimePicker1.Value;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox3.SelectedIndex > this.comboBox4.SelectedIndex)
            {
                MessageBox.Show("时间选择不正确!!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.comboBox3.SelectedIndex = this.comboBox4.SelectedIndex;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox3.SelectedIndex > this.comboBox4.SelectedIndex)
            {
                MessageBox.Show("时间选择不正确!!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.comboBox4.SelectedIndex = this.comboBox3.SelectedIndex;
            }
        }


    }
}
