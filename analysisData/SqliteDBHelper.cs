using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analysisData
{
    class SqliteDBHelper
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName"></param>
        public void createDB(String datasource)
        {
            SQLiteConnection.CreateFile("databases/"+datasource+ ".db");
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = "databases/" + datasource + ".db";
            conn.ConnectionString = connstr.ToString();
            conn.Open();
            //创建表
            SQLiteCommand cmd = new SQLiteCommand();
            string sql = "CREATE TABLE analysis(datea INTEGER,timea INTEGER,opena FLOAT ,higha FLOAT ,lowa FLOAT ,closea FLOAT ,vola INTEGER)";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            int i= cmd.ExecuteNonQuery();
            conn.Close();
        }
        /// <summary>
        /// 获得连接数据库
        /// </summary>
        /// <param name="datasource"></param>
        /// <returns></returns>
        public SQLiteConnection getSqlLiteDB(String datasource)
        {
            SQLiteConnection conn = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = "databases/" + datasource + ".db";
            conn.ConnectionString = connstr.ToString();
            return conn;
        }
        public int insertData(SQLiteCommand cmd, List<string> data)
        {
            string sql = "insert into analysis(datea,timea,opena,higha,lowa,closea,vola) values";
            foreach(string val in data){
               sql += "(" +val+ "),";
            }
            sql = sql.Substring(0,sql.Length-1);
            cmd.CommandText = sql;
           int i = cmd.ExecuteNonQuery();
           return i;
        }
        public List<AnalyBean> queryAnaly(SQLiteCommand cmd,string begd,string endd,string begt,string endt,int countType)
        {
            string sql = "";
            switch (countType)
            {// 0 分 1 时 2 天 3 周
                case 1:
                    sql = "select datea,timea,max(higha),max(lowa),count(higha)-count(lowa),timea/10000 ti from analysis where datea >=" + begd + " and datea <" + endd + " and timea >=" + begt + " and timea <" + endt + " group by ti";
                    break;
                case 2:
                    sql = "select datea,timea,max(higha),max(lowa),count(higha)-count(lowa) from analysis where datea >=" + begd + " and datea <" + endd + " and timea >=" + begt + " and timea <" + endt + " group by datea";
                    break;
                case 3:
                    sql = "select datea,timea,higha,lowa,higha-lowa from analysis where datea >=" + begd + " and datea <" + endd + " and timea >=" + begt + " and timea <" + endt ;
                    break;
                default:
                    sql = "select datea,timea,higha,lowa,higha-lowa from analysis where datea >=" + begd + " and datea <" + endd + " and timea >=" + begt + " and timea <" + endt ;
                    break;
            }

            //select datea,timea,count(higha),count(lowa) from
            cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            List<AnalyBean> lists = new List<AnalyBean>();
            while(reader.Read()){
                AnalyBean ab = new AnalyBean();
               ab.Datea = reader.GetInt32(0);
               ab.Timea = reader.GetInt32(1);
               ab.Higha = reader.GetFloat(2);
               ab.Lowa = reader.GetFloat(3);
                lists.Add(ab);
            }
            return lists;
        }

        /// <summary>
        /// 导入数据库
        /// </summary>
        /// <param name="fileName"></param>
        public void importDB(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            string datasource =file.Name.Substring(0, file.Name.IndexOf(file.Extension));
            //创建数据
            this.createDB(datasource);
            //获得数据连接
            SQLiteConnection conn = this.getSqlLiteDB(datasource);
            //执行导入
            lock(this){
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                long size = file.Length/55;
                FileStream stream=file.OpenRead();
                StreamReader streamReader = new StreamReader(stream);
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = streamReader.ReadLine();
                List<string> data=new List<string>();
                while((str = streamReader.ReadLine()) != null){
                    if(! str.Trim().Equals("")){
                        if (data.Count == 500)
                        {
                            this.insertData(cmd, data);
                            data.Clear();
                        }
                        data.Add(str.Trim().Substring(str.IndexOf(",")+1));
                        dataCount++;
                        this.progressNum = (int)(((double)this.dataCount / size) * 100);
   
                    }
                }
                this.insertData(cmd, data);
                conn.Close();
            }
        }

        private  int dataCount=0;
        private  int progressNum = 0;
        public int DataCount
        {
            get { return this.dataCount; }
            set { dataCount = value; }
        }
        public int ProgressNum
        {
            get { return this.progressNum; }
            set { progressNum = value; }
        }
       
    }
}
