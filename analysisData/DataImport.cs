using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace analysisData
{
    public partial class DataImport : Form
    {
        public DataImport()
        {
            InitializeComponent();
        }
        private string fileName;
        private SqliteDBHelper sqlitedb=new SqliteDBHelper();
        public string FileName
        {
            get { return this.fileName; }
            set { fileName = value; }
        }

        private void DataImport_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
            this.label4.Text = this.fileName;
            Thread thread = new Thread(new ThreadStart(import));
            thread.Start();
            

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void import() {
            sqlitedb.importDB(this.fileName);
       //     this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Value = sqlitedb.ProgressNum > 100 ? 100 : sqlitedb.ProgressNum;
            this.label2.Text = sqlitedb.DataCount +"条";
        }
    }
}
