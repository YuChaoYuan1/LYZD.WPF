using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using LYZD.Utility.Log;

namespace LYZD.DAL
{
    /// <summary>
    /// 通用数据存取层:Access数据库和SqlServer
    /// </summary>
    public class GeneralDal
    {
        #region 私有
        /// <summary>
        /// 表的列名称字典
        /// </summary>
        private Dictionary<string, List<FieldModel>> tableFields = new Dictionary<string, List<FieldModel>>();
        /// <summary>
        /// 连接字符串,仅在创立连接时赋值
        /// </summary>
        private readonly string connectionString = "";
        /// <summary>
        /// 写入错误日志
        /// </summary>
        private string errorString
        {
            set
            {

            }
        }
        #endregion
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connString">连接字符串</param>
        public GeneralDal(string connString)
        {
            connectionString = connString;
        }

        #region 基本查询
        /// 获取数据库中所有表的名称
        /// <summary>
        /// 获取数据库中所有表的名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableNames()
        {
            List<string> tableNames = new List<string>();
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    DataTable table = connection.GetSchema("Tables");
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["TABLE_TYPE"].ToString() == "TABLE" || row["TABLE_TYPE"].ToString() == "BASE TABLE")
                        {
                            tableNames.Add(row["TABLE_NAME"].ToString());
                        }
                    }
                }
                return tableNames;
            }
            catch (Exception e)
            {
                errorString = e.Message;
                return tableNames;
            }
        }
        /// <summary>
        /// 获取表所有列的名称
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <returns></returns>
        public List<FieldModel> GetFields(string tableName)
        {
            List<FieldModel> fields = new List<FieldModel>();
            try
            {
                if (tableFields.ContainsKey(tableName))
                {
                    return tableFields[tableName];
                }
                else
                {
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        string[] res = new string[] { null, null, tableName };
                        DataTable table = connection.GetSchema("Columns", res);
                        foreach (DataRow row in table.Rows)
                        {
                            string fieldName = row["COLUMN_NAME"].ToString();
                            string fieldType = row["DATA_TYPE"].ToString();

                            fields.Add(new FieldModel(fieldName, GetDataType(fieldType)));
                        }
                        tableFields.Add(tableName, fields);
                    }
                }
                return fields;
            }
            catch (Exception e)
            {
                //【标注】先注释
                LogManager.AddMessage(string.Format("获取表:{0} 的列名称列表错误.", tableName), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
                return fields;
            }
        }



        public DataTable GetAllTableData(string tableName, string sql = "")
        {
            DataTable table = new DataTable();
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand($"select * from {tableName}" , connection);
                    OleDbDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                    table.Load(reader);
                }
                return table;
            }
            catch (Exception e)
            {
                errorString = e.Message;
                return table;
            }

        }
        private EnumFieldDataType GetDataType(string idType)
        {
            EnumFieldDataType dataType = EnumFieldDataType.字符串;
            switch (idType)
            {
                case "130":
                    dataType = EnumFieldDataType.字符串;
                    break;
                case "3":

                case "131":
                    dataType = EnumFieldDataType.数值;
                    break;
                case "7":
                    dataType = EnumFieldDataType.时间;
                    break;
                default:
                    dataType = EnumFieldDataType.字符串;
                    break;
            }
            return dataType;
        }
        #endregion

        #region 增删改查
        /// 获取一条数据
        /// <summary>
        /// 获取一条数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="stringWhere">查询条件</param>
        /// <returns></returns>
        public DynamicModel GetByID(string tableName, string stringWhere = "")
        {
            //获取Sql语句
            string sql = string.Format("select * from {0}", tableName);
            if (!string.IsNullOrEmpty(stringWhere))
            {
                sql = string.Format("select * from {0} where {1}", tableName, stringWhere);
            }
            List<DynamicModel> models = GetModels(tableName, sql);
            if (models.Count > 0)
            {
                return models[0];
            }
            return null;
        }
        /// 根据sqlstring获取表格
        /// <summary>
        /// 根据sqlstring获取表格
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="stringWhere">查询条件</param>
        /// <returns></returns>
        public List<DynamicModel> GetList(string tableName, string stringWhere = "")
        {
            //获取Sql语句
            string sql = string.Format("select * from {0}", tableName);
            if (!string.IsNullOrEmpty(stringWhere))
            {
                sql = string.Format("select * from {0} where {1}", tableName, stringWhere);
            }
            return GetModels(tableName, sql);
        }
        /// <summary>
        /// 根据reader获取数据
        /// </summary>
        /// <param name="tableName">sql查询字符串</param>
        /// <param name="sql">表名称</param>
        /// <returns></returns>
        public List<DynamicModel> GetModels(string tableName, string sql)
        {
            List<DynamicModel> models = new List<DynamicModel>();
            try
            {
                List<FieldModel> fields = GetFields(tableName);
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand(sql, connection);
                    OleDbDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                    while (reader.Read())
                    {
                        DynamicModel model = new DynamicModel();
                        for (int i = 0; i < fields.Count; i++)
                        {
                            model.SetProperty(fields[i].FieldName, reader[fields[i].FieldName]);
                        }
                        models.Add(model);
                    }
                }
            }
            catch (Exception e)
            {
        
                LogManager.AddMessage(string.Format("查询数据库失败,sql语句为:{0}", sql), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return models;
        }
        /// <summary>
        /// 获取一页
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName">用于排序的字段</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="stringWhere"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public List<DynamicModel> GetPage(string tableName, string fieldName, int pageSize, int pageIndex, string stringWhere = "", bool isAsc = true)
        {
            //第0页不处理
            if (pageIndex == 0) return new List<DynamicModel>();
            #region 获取sql语句
            //"Example:select * from (select top 10 * from (select top 100 * from meter_info order by DTM_TEST_DATE ) AS a order by DTM_TEST_DATE desc) as b order by DTM_TEST_DATE;"
            string sql = "";
            if (isAsc)
            {
                if (string.IsNullOrEmpty(stringWhere))
                {
                    sql = string.Format("select * from (select top {2} * from (select top {3} * from {0} order by {1} ) AS a order by {1} desc) as b order by {1}", tableName, fieldName, pageSize, pageSize * pageIndex);
                }
                else
                {
                    sql = string.Format("select * from (select top {2} * from (select top {3} * from {0} where {4} order by {1} ) AS a order by {1} desc) as b order by {1}", tableName, fieldName, pageSize, pageSize * pageIndex, stringWhere);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(stringWhere))
                {
                    sql = string.Format("select * from (select top {2} * from (select top {3} * from {0} order by {1} desc) AS a order by {1}) as b order by {1} desc", tableName, fieldName, pageSize, pageSize * pageIndex);
                }
                else
                {
                    sql = string.Format("select * from (select top {2} * from (select top {3} * from {0} where {4} order by {1} desc) AS a order by {1} ) as b order by {1} desc", tableName, fieldName, pageSize, pageSize * pageIndex, stringWhere);
                }
            }
            #endregion
            return GetModels(tableName, sql);
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int GetCount(string tableName, string where = "")
        {
            int count = 0;
            string sql = string.Format("select count(*) from {0} where {1}", tableName, where);
           // string sql = @"SELECT COUNT(*) FROM T_LAST_INFO WHERE ";

            if (where == null || where == "")
            {
                sql = string.Format("select count(*) from {0}", tableName);
            }
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    DbCommand command = new OleDbCommand();
                    command = new OleDbCommand(sql, connection as OleDbConnection);
                    DbDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        count = (int)reader[0];
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("获取数据库中相关内容数量失败,sql语句为:{0}", sql), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return count;
        }
        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param>查询条件</param>
        /// <param name="model">要插入的数据模型</param>
        /// <returns>操作结果</returns>
        public int Insert(string tableName, DynamicModel model)
        {
            List<string> fields = model.GetAllProperyName();
            StringBuilder sbField = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            List<FieldModel> fieldModels = GetFields(tableName);
            for (int i = 0; i < fields.Count; i++)
            {
                if (model.GetProperty(fields[i]) == null || string.IsNullOrEmpty(model.GetProperty(fields[i]).ToString()))
                {
                    continue;
                }

                FieldModel fieldModel = fieldModels.Find(item => item.FieldName == fields[i]);
                if (fieldModel != null)
                {
                    //自增型不写入
                    if (fieldModel.FieldName.ToLower() == "id")
                    {
                        continue;
                    }
                    switch (fieldModel.DataType)
                    {
                        case EnumFieldDataType.字符串:
                            sbValue.Append(string.Format("'{0}',", model.GetProperty(fields[i])));
                            break;
                        case EnumFieldDataType.数值:
                            sbValue.Append(string.Format("{0},", model.GetProperty(fields[i])));
                            break;
                        default:
                            sbValue.Append(string.Format("'{0}',", model.GetProperty(fields[i])));
                            break;
                    }
                    sbField.Append(string.Format("{0},", fields[i]));
                }
            }
            sbField.Remove(sbField.Length - 1, 1);
            sbValue.Remove(sbValue.Length - 1, 1);
            string sql = string.Format("insert into {0} ({1}) values ({2})", tableName, sbField, sbValue);

            int count = 0;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand(sql, connection);
                    count = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("插入sql语句失败:{0}", e.Message), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return count;
        }

        public int Insert(string tableName, List<DynamicModel> models)
        {
            List<string> sqlList = new List<string>();
            for (int j = 0; j < models.Count; j++)
            {
                List<string> fields = models[j].GetAllProperyName();
                StringBuilder sbField = new StringBuilder();
                StringBuilder sbValue = new StringBuilder();
                List<FieldModel> fieldModels = GetFields(tableName);
                for (int i = 0; i < fields.Count; i++)
                {
                    if (models[j].GetProperty(fields[i]) == null || string.IsNullOrEmpty(models[j].GetProperty(fields[i]).ToString()))
                    {
                        continue;
                    }

                    FieldModel fieldModel = fieldModels.Find(item => item.FieldName == fields[i]);
                    if (fieldModel != null)
                    {
                        //自增型不写入
                        if (fieldModel.FieldName.ToLower() == "id")
                        {
                            continue;
                        }
                        switch (fieldModel.DataType)
                        {
                            case EnumFieldDataType.字符串:
                                sbValue.Append(string.Format("'{0}',", models[j].GetProperty(fields[i])));
                                break;
                            case EnumFieldDataType.数值:
                                sbValue.Append(string.Format("{0},", models[j].GetProperty(fields[i])));
                                break;
                            default:
                                sbValue.Append(string.Format("'{0}',", models[j].GetProperty(fields[i])));
                                break;
                        }
                        sbField.Append(string.Format("{0},", fields[i]));
                    }
                }
                sbField.Remove(sbField.Length - 1, 1);
                sbValue.Remove(sbValue.Length - 1, 1);
                string sql = string.Format("insert into {0} ({1}) values ({2})", tableName, sbField, sbValue);
                sqlList.Add(sql);
            }
            return ExecuteOperation(sqlList);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="where">更新条件</param>
        /// <param name="model">要更新的模型</param>
        /// <param name="fields">要更新的字段</param>
        /// <returns>操作结果</returns>
        public int Update(string tableName, string where, DynamicModel model, List<string> fields)
        {
            StringBuilder sb = new StringBuilder();
            List<FieldModel> fieldModels = GetFields(tableName);
            for (int i = 0; i < fields.Count; i++)
            {
                FieldModel fieldModel = fieldModels.Find(item => item.FieldName == fields[i]);
                if (fieldModel != null)
                {
                    switch (fieldModel.DataType)
                    {
                        case EnumFieldDataType.字符串:
                            sb.Append(string.Format(" [{0}] = '{1}',", fields[i], model.GetProperty(fields[i])));
                            break;
                        case EnumFieldDataType.数值:
                            sb.Append(string.Format(" [{0}] = {1},", fields[i], model.GetProperty(fields[i])));
                            break;
                        case EnumFieldDataType.时间:
                            var timeObj = model.GetProperty(fields[i]);
                            if (timeObj is DateTime)
                            {
                                sb.Append(string.Format(" [{0}] = #{1}#,", fields[i], ((DateTime)timeObj).ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                            break;
                        default:
                            sb.Append(string.Format(" [{0}] = '{1}',", fields[i], model.GetProperty(fields[i])));
                            break;
                    }
                }
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                return 0;
            }
            sb.Remove(sb.Length - 1, 1);
            string sql = string.Format("update {0} set {1} where {2}", tableName, sb, where);
            int count = 0;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand(sql, connection);
                    count = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("更新失败,sql语句为:{0}", sql), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return count;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="fieldName">作对比的列名称</param>
        /// <param name="models">要更新的模型</param>
        /// <param name="fields">要更新的字段</param>
        /// <returns>操作结果</returns>
        public int Update(string tableName, string fieldName, List<DynamicModel> models, List<string> fields)
        {
            List<FieldModel> fieldModels = GetFields(tableName);
            List<string> sqlList = new List<string>();
            for (int j = 0; j < models.Count; j++)
            {
                StringBuilder sb = new StringBuilder();
                DynamicModel model = models[j];
                #region 条件
                FieldModel fieldModelTemp = fieldModels.Find(item => item.FieldName == fieldName);
                string where = string.Format("{0}='{1}'", fieldName, model.GetProperty(fieldName));
                switch (fieldModelTemp.DataType)
                {
                    case EnumFieldDataType.数值:
                        where = string.Format(" [{0}] = {1}", fieldName, model.GetProperty(fieldName));
                        break;
                    case EnumFieldDataType.时间:
                        var timeObj = model.GetProperty(fieldName);
                        if (timeObj is DateTime)
                        {
                            where = string.Format(" [{0}] = #{1}#", fieldName, ((DateTime)timeObj).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        break;
                }
                #endregion
                #region 获取当前的sql语句
                for (int i = 0; i < fields.Count; i++)
                {
                    FieldModel fieldModel = fieldModels.Find(item => item.FieldName == fields[i]);
                    if (fieldModel != null)
                    {
                        switch (fieldModel.DataType)
                        {
                            case EnumFieldDataType.字符串:
                                sb.Append(string.Format(" [{0}] = '{1}',", fields[i], model.GetProperty(fields[i])));
                                break;
                            case EnumFieldDataType.数值:
                                sb.Append(string.Format(" [{0}] = {1},", fields[i], model.GetProperty(fields[i])));
                                break;
                            case EnumFieldDataType.时间:
                                var timeObj = model.GetProperty(fields[i]);
                                if (timeObj is DateTime)
                                {
                                    sb.Append(string.Format(" [{0}] = #{1}#,", fields[i], ((DateTime)timeObj).ToString("yyyy-MM-dd HH:mm:ss")));
                                }
                                break;
                            default:
                                sb.Append(string.Format(" [{0}] = '{1}',", fields[i], model.GetProperty(fields[i])));
                                break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    return 0;
                }
                sb.Remove(sb.Length - 1, 1);
                #endregion

                sqlList.Add(string.Format("update {0} set {1} where {2}", tableName, sb, where));
            }
            return ExecuteOperation(sqlList);
        }
        /// <summary>
        /// 执行一系列的sql语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns>受影响的行数</returns>
        public int ExecuteOperation(List<string> sqlList)
        {
            int count = 0;
            string sqlError = "";
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbTransaction transaction = connection.BeginTransaction();
                    OleDbCommand command = new OleDbCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    try
                    {
                        for (int i = 0; i < sqlList.Count; i++)
                        {
                            sqlError = sqlList[i];
                            command.CommandText = sqlList[i];
                            count += command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogManager.AddMessage(string.Format("批量执行失败,sql语句为:{0}", sqlError), EnumLogSource.数据库存取日志, EnumLevel.Error, ex);
                        count = 0;
                    }
                }
            }
            catch (Exception e)
            {
                count = 0;
                LogManager.AddMessage(string.Format("创建数据库连接失败", sqlError), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return count;
        }
        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete(string tableName, string where)
        {
            int count = 0;
            string sql = string.Format("delete from {0} where {1}", tableName, where);

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand(sql, connection);
                    count = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("删除数据失败,sql语句为{0}", sql), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return count;
        }
        /// 获取某一字段的所有值
        /// <summary>
        /// 获取某一字段的所有值
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="where">查询条件</param>
        /// <param name="topCount">选择多少条数据</param>
        /// <param name="isAsc">升序还是降序</param>
        /// <returns>获取到的值列表</returns>
        public List<string> GetDistinct(string tableName, string fieldName, string where = "",int topCount=0,bool isAsc=true)
        {
            List<string> valueList = new List<string>();
            string sql = string.Format("select distinct {1} from {0}", tableName, fieldName);
            if (topCount > 0)
            {
                if (isAsc)
                {
                    sql = string.Format("select distinct top {3} {1} from {0} order by {1} asc", tableName, fieldName, where, topCount);
                }
                else
                {
                    sql = string.Format("select distinct top {3} {1} from {0} order by {1} desc", tableName, fieldName, where, topCount);
                }
            }
            if (!string.IsNullOrEmpty(where))
            {
                sql = string.Format("select distinct {1} from {0} where {2}", tableName, fieldName, where);
                if (topCount > 0)
                {
                    if (isAsc)
                    {
                        sql = string.Format("select distinct top {3} {1} from {0} where {2} order by {1} asc", tableName, fieldName, where,topCount);
                    }
                    else
                    {
                        sql = string.Format("select distinct top {3} {1} from {0} where {2} order by {1} desc", tableName, fieldName, where, topCount);
                    }
                }
            }
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand(sql, connection);
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        valueList.Add(reader[0].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("查询数据库失败:{0}. sql语句为:{1}", e.Message, sql), EnumLogSource.数据库存取日志, EnumLevel.Error, e);
            }
            return valueList;
        }
        /// 根据自定义的列名称获取数据列表
        /// <summary>
        /// 根据自定义的列名称获取数据列表
        /// </summary>
        /// <param name="sql">sql查询语句</param>
        /// <param name="fields">列名称列表</param>
        /// <returns></returns>
        public List<DynamicModel> GetList(string sql, List<string> fields)
        {
            List<DynamicModel> models = new List<DynamicModel>();
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand(sql, connection);
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DynamicModel model = new DynamicModel();
                        for (int i = 0; i < fields.Count; i++)
                        {
                            model.SetProperty(fields[i], reader[fields[i]]);
                        }
                        models.Add(model);
                    }
                }
            }
            catch (Exception e)
            {
                errorString = string.Format("查询数据库失败:{0}. sql语句为:{1}", e.Message, sql);
            }
            return models;
        }

        /// <summary>
        /// 根据一系列Sql语句获取模型列表
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        public List<DynamicModel> GetList(List<string> tableNames, List<string> sqlList)
        {
            List<DynamicModel> models = new List<DynamicModel>();
            if (sqlList == null)
            { return models; }
            string sql = "";
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    for (int i = 0; i < sqlList.Count; i++)
                    {
                        sql = sqlList[i];
                        string tableName = tableNames[i];

                        List<FieldModel> fields = GetFields(tableName);
                        OleDbCommand command = new OleDbCommand(sql, connection);
                        OleDbDataReader reader = command.ExecuteReader(CommandBehavior.Default);
                        while (reader.Read())
                        {
                            DynamicModel model = new DynamicModel();
                            for (int j = 0; j < fields.Count; j++)
                            {
                                model.SetProperty(fields[j].FieldName, reader[fields[j].FieldName]);
                            }
                            models.Add(model);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorString = string.Format("查询数据库失败:{0}. sql语句为:{1}", e.Message, sql);
            }
            return models;
        }
        #endregion
    }
}
