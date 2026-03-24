using System.Data;
using System.Data.SqlClient;
using LYZD.DAL.Config;

namespace LYZD.Mis.Common
{
    /// <summary>
	/// 数据访问基础类(基于SQL server)
	/// </summary>
    internal abstract class SqlHelper
    {
        //数据库连接字符串，可以动态更改StrConnectString支持多数据库.        
        public SqlHelper()
        {
            //App.SystemConfig.Load();
        }
        private static string StrConnectString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Persist Security Info=True",
                 ConfigHelper.Instance.Marketing_DataSource,
                 "",   //TODO2设置里面加上目录
                 ConfigHelper.Instance.Marketing_UserName,
                 ConfigHelper.Instance.Marketing_UserPassWord);



        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string sql)
        {
            using (SqlConnection connection = new SqlConnection(StrConnectString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataTable Query(string sql)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(StrConnectString))
            {
                conn.Open();
                SqlDataAdapter command = new SqlDataAdapter(sql, conn);
                command.Fill(dt);
                conn.Close();

            }
            return dt;
        }


    }
}
