using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.Core
{
	public class AccessHelper
	{
		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		private static string connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\\DataBase\\AppData.mdb", Directory.GetCurrentDirectory());
		private static System.Data.OleDb.OleDbCommand com;
		private static System.Data.OleDb.OleDbDataReader reader;
		private static System.Data.OleDb.OleDbDataAdapter adapter;
		private static System.Data.OleDb.OleDbConnection conn;
		public static OleDbConnection NewConn
		{
			get
			{
				string connectionString = null;
				//connectionString = System.Configuration.ConfigurationSettings.GetConfig("Supermarket")
				//连接2010数据库
				//connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\GCU.accdb"
				//连接03-07数据库
				connectionString = connString;

				//应该在这里先判断conn是否为Nothing
				if (conn == null)
				{
					conn = new System.Data.OleDb.OleDbConnection(connectionString);
				}
				if (conn.State != ConnectionState.Open)
				{
					conn.Open();
				}
				return conn;
			}
		}
		public static DataTable GetDataTable(string sql)
		{
			DataSet dataset = default(DataSet);
			dataset = new DataSet();
			com = new System.Data.OleDb.OleDbCommand(sql, NewConn);
			adapter = new OleDbDataAdapter(com);
			adapter.Fill(dataset);
			return dataset.Tables[0];
		}
	}
}
