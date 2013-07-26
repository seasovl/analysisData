/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 * 
 * Released to the public domain, use at your own risk!
 ********************************************************/

using System;
using System.Data.Common;
using System.Diagnostics;
using System.Text;


namespace test
{
    class Program
    {
        [STAThread()]
        static void Main1(string[] args)
        {

            string datasource = "test.db";

            System.Data.SQLite.SQLiteConnection.CreateFile(datasource);

            //连接数据库

            System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection();

            System.Data.SQLite.SQLiteConnectionStringBuilder connstr =

                new System.Data.SQLite.SQLiteConnectionStringBuilder();

            connstr.DataSource = datasource;

            connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
            Console.WriteLine(connstr.ToString());
            conn.ConnectionString = connstr.ToString();

            conn.Open();

            //创建表

            System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();

            string sql = "CREATE TABLE test(username varchar(20),password varchar(20))";

            cmd.CommandText = sql;

            cmd.Connection = conn;

            cmd.ExecuteNonQuery();

            //插入数据

            sql = "INSERT INTO test VALUES('dotnetthink','mypassword')";

            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();

            //取出数据

            sql = "SELECT * FROM test";

            cmd.CommandText = sql;

            System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader();

            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
                sb.Append("username:").Append(reader.GetString(0)).Append("\n").Append("password:").Append(reader.GetString(1));

            }
            Console.WriteLine(sb.ToString());

        }

       [STAThread()]
        static void Main(string[] args){
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=test.db;password=admin";
                cnn.Open();
              DbCommand commd= cnn.CreateCommand();

               commd.CommandText="select * from test ";
               DbDataReader reader = commd.ExecuteReader();

               StringBuilder sb = new StringBuilder();

               while (reader.Read())
               {
                   sb.Append("username:").Append(reader.GetString(0)).Append("\n").Append("password:").Append(reader.GetString(1));

               }
               Console.WriteLine(sb.ToString());
            }
        }
    }
}
