using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
//using System.Data.OracleClient;

namespace LYZD.Mis.Common
{
    public class OracleHelper
    {
        public OracleHelper(string ip, int port, string dataSource, string userId, string pwd, string webUrl)
        {
            this.Ip = ip;
            this.Port = port;
            this.DataSource = dataSource;
            this.UserId = userId;
            this.Password = pwd;
            this.WebServiceURL = webUrl;
        }

        public OracleHelper()
        {

        }


        #region 属性
        /// <summary>
        /// 数据库IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 数据库端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 数据库登陆用户
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 数据库登陆密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// WebServer路径
        /// </summary>
        public string WebServiceURL { get; set; }

        /// <summary>
        /// 获取当前联接字符串
        /// </summary>
        public string ConnectString
        {
            get
            {
                return string.Format("Data Source=(DESCRIPTION =(ADDRESS =(PROTOCOL = TCP)(HOST = {0})(PORT = {1}))(CONNECT_DATA = (SERVICE_NAME ={2})));User ID={3};Password={4};Persist Security Info=True",
                        Ip, Port, DataSource, UserId, Password);
            }
        }
        #endregion

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlList">多条SQL语句</param>
        public void Execute(List<string> sqlList)
        {
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {

                conn.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    OracleTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;

                    foreach (string s in sqlList)
                    {
                        if (string.IsNullOrEmpty(s)) continue;
                        if (s.Length < 5) continue;
                        cmd.CommandText = s;
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                conn.Close();

            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlList">多条SQL语句</param>
        public void ExecuteOne(string sqlList)
        {
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    OracleTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    
                    if (string.IsNullOrEmpty(sqlList)) 
                    if (sqlList.Length < 5)
                    {

                    }
                    cmd.CommandText = sqlList;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                conn.Close();

            }
        }


        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public DataTable ExecuteReader(string sql)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                OracleDataAdapter adp = new OracleDataAdapter(sql, conn);
                adp.Fill(dt);
                adp.Dispose();
                conn.Close();
            }
            return dt;
        }
        private static object Lock = new object();
        public  DataSet QueryFa(string SQLString)
        {
            DataSet result;
            lock (Lock)
            {
                using (OracleConnection conn = new OracleConnection(ConnectString))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        conn.Open();
                        OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(SQLString, conn);
                        oracleDataAdapter.Fill(dataSet, "ds");
                        oracleDataAdapter.Dispose();
                        conn.Close();
                    }
                    catch (OracleException ex)
                    {
                        //LogInfoHelper.Write(ex);
                       // throw new System.Exception(ex.Message);
                    }
                    result = dataSet;
                }
            }
            return result;
        }
        public  DataSet Query(string SQLString)
        {
            DataSet result;
            lock (Lock)
            {
                using (OracleConnection conn = new OracleConnection(ConnectString))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        conn.Open();
                        OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(SQLString, conn);
                        oracleDataAdapter.Fill(dataSet, "ds");
                        oracleDataAdapter.Dispose();
                        conn.Close();
                    }
                    catch (OracleException ex)
                    {
                        //LogInfoHelper.Write(ex);
                        //throw new System.Exception(ex.Message);
                    }
                    result = dataSet;
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
            object o = null;
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    o = cmd.ExecuteScalar();
                    conn.Close();
                }
                conn.Close();
            }
            return o;

        }

        public int ExecuteNonQuery(string sql)
        {
            int count = 0;
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    count = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return count;
        }


    }
}
