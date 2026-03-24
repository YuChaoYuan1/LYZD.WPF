using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseInitialized
{
    class Universal
    {
        public event EventHandler<string> OutMessage;
        public string ConnectionString
        {
            get
            {
                if (8 == IntPtr.Size)
                {
                    return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetCurrentDirectory()}\DataBase\AppData.mdb";
                }
                else
                {
                    return $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Directory.GetCurrentDirectory()}\DataBase\AppData.mdb";
                }
            }
        }
        public string ConnectionStringMeterData
        {
            get
            {
                if (8 == IntPtr.Size)
                {
                    return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetCurrentDirectory()}\DataBase\MeterData.mdb";
                }
                else
                {
                    return $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Directory.GetCurrentDirectory()}\DataBase\MeterData.mdb";
                }
            }
        }
        public string ConnectionStringTmpMeterData
        {
            get
            {
                if (8 == IntPtr.Size)
                {
                    return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetCurrentDirectory()}\DataBase\TmpMeterData.mdb";
                }
                else
                {
                    return $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Directory.GetCurrentDirectory()}\DataBase\TmpMeterData.mdb";
                }
            }
        }
        public virtual void Execute()
        {
            try
            {

                Add_T_CODE_TREE("LY数据服务", "", "3", "MisType");
                ShowMsg("初始化营销数据服务完成");
                Add_T_CODE_TREE("通电检测", "", "3", "PrepareTest", "03");
                Add_T_CODE_TREE("通电检测", "", "2", "MeterResultViewId", "00003");
                Add_T_SCHEMA_PARA_FORMAT("00003", "通电检测", "", "", "", "", "00003", "", "AAPrepareTest.PowerOnControlCheckAddress", "1", "", "698;376.1");
                Add_T_VIEW_CONFIG("00003", "通电检测", "METER_COMMUNICATION", "<698|读取地址|核对地址|698>,结论,分项结论376|分项结论698", "MD_VALUE,MD_RESULT,MD_ITEM_VALUE");
                Add_T_SCHEMA_PARA_FORMAT("13007", "常温基本误差", "VoltageRatio|VoltageRatio|VoltageRatio|CurrentRatio|CurrentRatio|CurrentRatio|PowerFactor|PowerDirection", "A相电压比例|B相电压比例|C相电压比例|A相电流比例|B相电流比例|C相电流比例|功率因素|功率方向", "true|true|true|true|true|true|true|true", "true|true|true|true|true|true|true|true", "13007", "100%|100%|100%|100%|100%|100%|1.0|正向有功", "AccurateMeasurementTest.BaseNormalTemperatureError", "1", "", "698;376.1");
                Add_T_SCHEMA_PARA_FORMAT("13010", "电源影响", "VoltageRatio|VoltageRatio|VoltageRatio|CurrentRatio|CurrentRatio|CurrentRatio|PowerFactor|PowerDirection", "A相电压比例|B相电压比例|C相电压比例|A相电流比例|B相电流比例|C相电流比例|功率因素角度|功率方向", "true|true|true|true|true|true|true|true", "true|true|true|true|true|true|true|true", "13010", "100%|100%|100%|100%|100%|100%|1.0|正向有功", "AccurateMeasurementTest.BasePowerImpactError", "1", "", "698;376.1");
                ShowMsg("初始化检定项目与结论完成");
                Add_T_CONFIG_PARA_FORMAT("02010", "功耗表位|二次回路表位|供电时间", "||", "0|0|120");
                Add_T_CONFIG_PARA_VALUE("02010", "0|0|120");


                ShowMsg("软件配置");
                ShowMsg("智慧工控平台");
                Add_T_CODE_TREE("智慧工控平台", "", "3", "MisType", "11");
                Add_T_CODE_TREE("工控平台上报", "", "3", "NetworkInformation", "02");
                Add_T_CONFIG_PARA_FORMAT("05002", "是否启用|工况信息上报IP|工况信息上报端口|上报频率(s)|设备编号|工控信息接收端口|是否人工台", "YesNo||||||YesNo", "是|192.168.100.10|44309|60||44300|是");
                ShowMsg("初始化检定配置完成");
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }

        private List<string> GetColumnNames(string connectionString, string tableName)
        {
            List<string> ColumnNames = new List<string>();
            if (string.IsNullOrWhiteSpace(tableName)) return ColumnNames;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    DataTable table = connection.GetSchema("Columns", new string[] { null, null, tableName });
                    foreach (DataRow row in table.Rows)
                    {
                        string fieldName = row["COLUMN_NAME"].ToString();
                        ColumnNames.Add(fieldName);
                    }
                }
            }
            catch { }
            return ColumnNames;
        }

        public void ShowMsg(string msg)
        {
            if (OutMessage != null)
            {
                OutMessage.Invoke(this, msg);
            }
        }

        #region ADD 
        protected internal void Add_T_CODE_TREE(string code_cn_name, string code_en_name, string code_level, string code_parent, string code_value = "", string code_enabled = "1")
        {
            bool increase_code_value = false;
            if (string.IsNullOrWhiteSpace(code_value))
            {
                increase_code_value = true;
            }
            int target_code_value = 0;

            bool exist;
            bool update = true;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                string sql = $"select ID,CODE_CN_NAME,CODE_EN_NAME,CODE_LEVEL,CODE_PARENT,CODE_VALUE,CODE_ENABLED from T_CODE_TREE where CODE_PARENT='{code_parent}' and CODE_LEVEL='{code_level}' ";
                if (!string.IsNullOrWhiteSpace(code_value))
                {
                    sql += $" and CODE_VALUE='{code_value}' ";
                }
                command.CommandText = sql;
                OleDbDataReader reader = command.ExecuteReader();
                exist = false;
                while (reader.Read())
                {
                    if (increase_code_value)
                    {
                        int.TryParse(reader["CODE_VALUE"].ToString(), out int tmp_code_value);
                        if (target_code_value <= tmp_code_value)
                        {
                            target_code_value = tmp_code_value + 1;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(code_value))
                    {
                        exist = true;
                        if (code_cn_name == reader["CODE_CN_NAME"].ToString())
                        {
                            update = false;
                        }
                    }
                    else
                    {
                        update = false;//code_value空时不能更新
                        if (code_cn_name == reader["CODE_CN_NAME"].ToString())
                        {
                            exist = true;
                        }

                    }
                }
                reader.Close();
            }
            if (exist)
            {
                if (update)
                {
                    Update_T_CODE_TREE(code_cn_name, code_en_name, code_level, code_parent, code_value, code_enabled);
                }
            }
            else
            {
                if (increase_code_value)
                {
                    code_value = target_code_value.ToString();
                }
                Insert_T_CODE_TREE(code_cn_name, code_en_name, code_level, code_parent, code_value, code_enabled);
            }

        }


        protected internal void Add_T_CONFIG_PARA_FORMAT(string config_no, string config_view, string config_code, string config_default_value)
        {
            bool exist;
            bool update = true;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"select CONFIG_NO,CONFIG_VIEW,CONFIG_CODE,CONFIG_DEFAULT_VALUE from T_CONFIG_PARA_FORMAT where CONFIG_NO='{config_no}';";
                OleDbDataReader reader = command.ExecuteReader();
                exist = reader.HasRows;
                if (reader.Read())
                {
                    if (config_view == reader["CONFIG_VIEW"].ToString()
                    && config_code == reader["CONFIG_CODE"].ToString()
                    && config_default_value == reader["CONFIG_DEFAULT_VALUE"].ToString())
                    {
                        update = false;
                    }
                }
                reader.Close();
            }
            if (exist)
            {
                if (update)
                {
                    Update_T_CONFIG_PARA_FORMAT(config_no, config_view, config_code, config_default_value);
                }
            }
            else
            {
                Insert_T_CONFIG_PARA_FORMAT(config_no, config_view, config_code, config_default_value);
            }
        }
        protected internal string Get_T_CONFIG_PARA_VALUE(string config_no)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"select ID,CONFIG_NO,CONFIG_VALUE from T_CONFIG_PARA_VALUE where CONFIG_NO='{config_no}';";
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return reader["CONFIG_VALUE"].ToString();
                }
                reader.Close();
            }
            return null;
        }
        protected internal void Add_T_CONFIG_PARA_VALUE(string config_no, string config_value)
        {
            bool exist;
            bool update = true;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"select ID,CONFIG_NO,CONFIG_VALUE from T_CONFIG_PARA_VALUE where CONFIG_NO='{config_no}';";
                OleDbDataReader reader = command.ExecuteReader();
                exist = reader.HasRows;

                if (reader.Read())
                {
                    if (config_value == reader["CONFIG_VALUE"].ToString())
                    {
                        update = false;
                    }
                }
                reader.Close();
            }
            if (exist)
            {
                if (update)
                {
                    Update_T_CONFIG_PARA_VALUE(config_no, config_value);
                }
            }
            else
            {
                Insert_T_CONFIG_PARA_VALUE(config_no, config_value);
            }
        }

        protected internal void Add_T_SCHEMA_PARA_FORMAT(string para_no, string para_name, string para_p_code, string para_view, string para_key_rule, string para_view_rule, string result_view_id, string default_value, string check_class_name, string para_name_rule, string default_sort_no, string para_apply)
        {
            bool exist;
            bool update = true;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT [PARA_NO], [PARA_NAME], [PARA_P_CODE], [PARA_VIEW], [PARA_KEY_RULE], [PARA_VIEW_RULE], [RESULT_VIEW_ID], [DEFAULT_VALUE], [CHECK_CLASS_NAME], [PARA_NAME_RULE], [DEFAULT_SORT_NO], [PARA_APPLY] FROM T_SCHEMA_PARA_FORMAT where PARA_NO='{para_no}'";

                OleDbDataReader reader = command.ExecuteReader();
                exist = reader.HasRows;
                if (reader.Read())
                {
                    if (para_name == reader["PARA_NAME"].ToString()
                    && para_p_code == reader["PARA_P_CODE"].ToString()
                    && para_view == reader["PARA_VIEW"].ToString()
                    && para_key_rule == reader["PARA_KEY_RULE"].ToString()
                    && para_view_rule == reader["PARA_VIEW_RULE"].ToString()
                    && result_view_id == reader["RESULT_VIEW_ID"].ToString()
                    && default_value == reader["DEFAULT_VALUE"].ToString()
                    && check_class_name == reader["CHECK_CLASS_NAME"].ToString()
                    && para_name_rule == reader["PARA_NAME_RULE"].ToString()
                    && default_sort_no == reader["DEFAULT_SORT_NO"].ToString()
                    && para_apply == reader["PARA_APPLY"].ToString()
                    )
                    {
                        update = false;
                    }
                }
                reader.Close();
            }
            if (exist)
            {
                if (update)
                {
                    Update_T_SCHEMA_PARA_FORMAT(para_no, para_name, para_p_code, para_view, para_key_rule, para_view_rule, result_view_id, default_value, check_class_name, para_name_rule, default_sort_no, para_apply);
                }
            }
            else
            {
                Insert_T_SCHEMA_PARA_FORMAT(para_no, para_name, para_p_code, para_view, para_key_rule, para_view_rule, result_view_id, default_value, check_class_name, para_name_rule, default_sort_no, para_apply);
            }
        }

        protected internal void Add_T_VIEW_CONFIG(string avr_view_id, string avr_check_name, string avr_table_name, string avr_col_show_name, string avr_col_name)
        {
            bool exist;
            bool update = true;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT [AVR_VIEW_ID],[AVR_CHECK_NAME],[AVR_TABLE_NAME],[AVR_COL_SHOW_NAME],[AVR_COL_NAME] FROM T_VIEW_CONFIG where AVR_VIEW_ID='{avr_view_id}' and AVR_CHECK_NAME='{avr_check_name}'";

                OleDbDataReader reader = command.ExecuteReader();
                exist = reader.HasRows;
                if (reader.Read())
                {
                    if (avr_table_name.Equals(reader["AVR_TABLE_NAME"].ToString())
                    && avr_col_show_name.Equals(reader["AVR_COL_SHOW_NAME"].ToString())
                    && avr_col_name.Equals(reader["AVR_COL_NAME"].ToString())
                    )
                    {
                        update = false;
                    }
                }
                reader.Close();
            }
            if (exist)
            {
                if (update)
                {
                    Update_T_VIEW_CONFIG(avr_view_id, avr_check_name, avr_table_name, avr_col_show_name, avr_col_name);
                }
            }
            else
            {
                Insert_T_VIEW_CONFIG(avr_view_id, avr_check_name, avr_table_name, avr_col_show_name, avr_col_name);
            }
        }

        protected internal void Add_T_USER_INFO(string user_id, string user_name, string user_password, string user_power)
        {

            bool exist;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT [USER_ID], [USER_NAME], [USER_PASSWORD], [USER_POWER] FROM T_USER_INFO where USER_ID = '{user_id}' or USER_NAME = '{user_name}'";

                OleDbDataReader reader = command.ExecuteReader();
                exist = reader.HasRows;

                reader.Close();
            }
            if (exist)
            {

            }
            else
            {
                Insert_T_USER_INFO(user_id, user_name, user_password, user_power);
            }
        }

        protected internal void Add_T_MENU_VIEW(string menu_name, string menu_class, string menu_image, string valid_flag, string menu_datasource, string menu_check_enable, string menu_user_visible, string manu_main, string menu_category, string sort_id)
        {
            bool exist;
            bool update = true;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT [ID],[MENU_NAME],[MENU_CLASS],[MENU_IMAGE],[VALID_FLAG],[MENU_DATASOURCE],[MENU_CHECK_ENABLE],[MENU_USER_VISIBLE],[MENU_MAIN],[MENU_CATEGORY],[SORT_ID] FROM T_MENU_VIEW where MENU_NAME='{menu_name}'";

                OleDbDataReader reader = command.ExecuteReader();
                exist = reader.HasRows;
                if (reader.Read())
                {
                    update = false;
                }
                reader.Close();
            }
            if (exist)
            {
                if (update)
                {
                    Update_T_MENU_VIEW(menu_name, menu_class, menu_image, valid_flag, menu_datasource, menu_check_enable, menu_user_visible, manu_main, menu_category, sort_id);
                }
            }
            else
            {
                Insert_T_MENU_VIEW(menu_name, menu_class, menu_image, valid_flag, menu_datasource, menu_check_enable, menu_user_visible, manu_main, menu_category, sort_id);
            }
        }

        #endregion

        #region CURD
        private void Insert_T_CODE_TREE(string target_code_cn_name, string target_code_en_name, string target_code_level, string target_code_parent, string target_code_value, string target_code_enabled)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_CODE_TREE(CODE_CN_NAME,CODE_EN_NAME,CODE_LEVEL,CODE_PARENT,CODE_VALUE,CODE_ENABLED) values(@CODE_CN_NAME,@CODE_EN_NAME,@CODE_LEVEL,@CODE_PARENT,@CODE_VALUE,@CODE_ENABLED);";

                command.Parameters.Add(new OleDbParameter("CODE_CN_NAME", target_code_cn_name));
                command.Parameters.Add(new OleDbParameter("CODE_EN_NAME", target_code_en_name));
                command.Parameters.Add(new OleDbParameter("CODE_LEVEL", target_code_level));
                command.Parameters.Add(new OleDbParameter("CODE_PARENT", target_code_parent));
                command.Parameters.Add(new OleDbParameter("CODE_VALUE", target_code_value));
                command.Parameters.Add(new OleDbParameter("CODE_ENABLED", target_code_enabled));

                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Update_T_CODE_TREE(string target_code_cn_name, string target_code_en_name, string target_code_level, string target_code_parent, string target_code_value, string target_code_enabled)
        {
            if (string.IsNullOrWhiteSpace(target_code_level)
                || string.IsNullOrWhiteSpace(target_code_parent)
                || string.IsNullOrWhiteSpace(target_code_value))
            {
                return;
            }
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"update T_CODE_TREE set CODE_CN_NAME=@CODE_CN_NAME,CODE_EN_NAME=@CODE_EN_NAME,CODE_ENABLED=@CODE_ENABLED where CODE_LEVEL='{target_code_level}' and CODE_PARENT='{target_code_parent}' and CODE_VALUE='{target_code_value}';";

                command.Parameters.Add(new OleDbParameter("CODE_CN_NAME", target_code_cn_name));
                command.Parameters.Add(new OleDbParameter("CODE_EN_NAME", target_code_en_name));
                command.Parameters.Add(new OleDbParameter("CODE_LEVEL", target_code_level));
                command.Parameters.Add(new OleDbParameter("CODE_PARENT", target_code_parent));
                command.Parameters.Add(new OleDbParameter("CODE_VALUE", target_code_value));
                command.Parameters.Add(new OleDbParameter("CODE_ENABLED", target_code_enabled));

                int row = command.ExecuteNonQuery();
                ShowMsg($"更新{row}行");
            }
        }

        private void Insert_T_CONFIG_PARA_FORMAT(string config_no, string config_view, string config_code, string config_default_value)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_CONFIG_PARA_FORMAT(CONFIG_NO,CONFIG_VIEW,CONFIG_CODE,CONFIG_DEFAULT_VALUE) values(@CONFIG_NO,@CONFIG_VIEW,@CONFIG_CODE,@CONFIG_DEFAULT_VALUE);";

                command.Parameters.Add(new OleDbParameter("CONFIG_NO", config_no));
                command.Parameters.Add(new OleDbParameter("CONFIG_VIEW", config_view));
                command.Parameters.Add(new OleDbParameter("CONFIG_CODE", config_code));
                command.Parameters.Add(new OleDbParameter("CONFIG_DEFAULT_VALUE", config_default_value));

                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Update_T_CONFIG_PARA_FORMAT(string config_no, string config_view, string config_code, string config_default_value)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"update T_CONFIG_PARA_FORMAT set CONFIG_VIEW=@CONFIG_VIEW,CONFIG_CODE=@CONFIG_CODE,CONFIG_DEFAULT_VALUE=@CONFIG_DEFAULT_VALUE where CONFIG_NO='{config_no}';";

                command.Parameters.Add(new OleDbParameter("CONFIG_VIEW", config_view));
                command.Parameters.Add(new OleDbParameter("CONFIG_CODE", config_code));
                command.Parameters.Add(new OleDbParameter("CONFIG_DEFAULT_VALUE", config_default_value));

                int row = command.ExecuteNonQuery();
                ShowMsg($"更新{row}行");
            }
        }

        private void Insert_T_CONFIG_PARA_VALUE(string target_config_no, string target_config_value)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_CONFIG_PARA_VALUE(CONFIG_NO,CONFIG_VALUE) values(@CONFIG_NO,@CONFIG_VALUE);";

                command.Parameters.Add(new OleDbParameter("CONFIG_NO", target_config_no));
                command.Parameters.Add(new OleDbParameter("CONFIG_VALUE", target_config_value));

                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Update_T_CONFIG_PARA_VALUE(string target_config_no, string target_config_value)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"update T_CONFIG_PARA_VALUE set CONFIG_VALUE=@CONFIG_VALUE where CONFIG_NO='{target_config_no}';";

                command.Parameters.Add(new OleDbParameter("CONFIG_VALUE", target_config_value));

                int row = command.ExecuteNonQuery();
                ShowMsg($"更新{row}行");
            }
        }

        private void Insert_T_SCHEMA_PARA_FORMAT(string para_no, string para_name, string para_p_code, string para_view, string para_key_rule, string para_view_rule, string result_view_id, string default_value, string check_class_name, string para_name_rule, string default_sort_no, string para_apply)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_SCHEMA_PARA_FORMAT([PARA_NO], [PARA_NAME], [PARA_P_CODE], [PARA_VIEW], [PARA_KEY_RULE], [PARA_VIEW_RULE], [RESULT_VIEW_ID], [DEFAULT_VALUE], [CHECK_CLASS_NAME], [PARA_NAME_RULE], [DEFAULT_SORT_NO],[PARA_APPLY]) values(@PARA_NO,@PARA_NAME,@PARA_P_CODE,@PARA_VIEW,@PARA_KEY_RULE,@PARA_VIEW_RULE,@RESULT_VIEW_ID,@DEFAULT_VALUE,@CHECK_CLASS_NAME,@PARA_NAME_RULE,@DEFAULT_SORT_NO,@PARA_APPLY);";

                command.Parameters.Add(new OleDbParameter("PARA_NO", para_no));
                command.Parameters.Add(new OleDbParameter("PARA_NAME", para_name));
                command.Parameters.Add(new OleDbParameter("PARA_P_CODE", para_p_code));
                command.Parameters.Add(new OleDbParameter("PARA_VIEW", para_view));
                command.Parameters.Add(new OleDbParameter("PARA_KEY_RULE", para_key_rule));
                command.Parameters.Add(new OleDbParameter("PARA_VIEW_RULE", para_view_rule));
                command.Parameters.Add(new OleDbParameter("RESULT_VIEW_ID", result_view_id));
                command.Parameters.Add(new OleDbParameter("DEFAULT_VALUE", default_value));
                command.Parameters.Add(new OleDbParameter("CHECK_CLASS_NAME", check_class_name));
                command.Parameters.Add(new OleDbParameter("PARA_NAME_RULE", para_name_rule));
                command.Parameters.Add(new OleDbParameter("DEFAULT_SORT_NO", default_sort_no));
                command.Parameters.Add(new OleDbParameter("PARA_APPLY", para_apply));
                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Update_T_SCHEMA_PARA_FORMAT(string para_no, string para_name, string para_p_code, string para_view, string para_key_rule, string para_view_rule, string result_view_id, string default_value, string check_class_name, string para_name_rule, string default_sort_no, string para_apply)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"update T_SCHEMA_PARA_FORMAT set [PARA_NAME]=@PARA_NAME, [PARA_P_CODE]=@PARA_P_CODE, [PARA_VIEW]=@PARA_VIEW, [PARA_KEY_RULE]=@PARA_KEY_RULE, [PARA_VIEW_RULE]=@PARA_VIEW_RULE, [RESULT_VIEW_ID]=@RESULT_VIEW_ID, [DEFAULT_VALUE]=@DEFAULT_VALUE, [CHECK_CLASS_NAME]=@CHECK_CLASS_NAME, [PARA_NAME_RULE]=@PARA_NAME_RULE, [DEFAULT_SORT_NO]=@DEFAULT_SORT_NO  where [PARA_NO]='{para_no}';";

                command.Parameters.Add(new OleDbParameter("PARA_NAME", para_name));
                command.Parameters.Add(new OleDbParameter("PARA_P_CODE", para_p_code));
                command.Parameters.Add(new OleDbParameter("PARA_VIEW", para_view));
                command.Parameters.Add(new OleDbParameter("PARA_KEY_RULE", para_key_rule));
                command.Parameters.Add(new OleDbParameter("PARA_VIEW_RULE", para_view_rule));
                command.Parameters.Add(new OleDbParameter("RESULT_VIEW_ID", result_view_id));
                command.Parameters.Add(new OleDbParameter("DEFAULT_VALUE", default_value));
                command.Parameters.Add(new OleDbParameter("CHECK_CLASS_NAME", check_class_name));
                command.Parameters.Add(new OleDbParameter("PARA_NAME_RULE", para_name_rule));
                command.Parameters.Add(new OleDbParameter("DEFAULT_SORT_NO", default_sort_no));
                command.Parameters.Add(new OleDbParameter("PARA_APPLY", para_apply));
                int row = command.ExecuteNonQuery();
                ShowMsg($"更新{row}行");
            }
        }

        private void Insert_T_VIEW_CONFIG(string avr_view_id, string avr_check_name, string avr_table_name, string avr_col_show_name, string avr_col_name)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_VIEW_CONFIG([AVR_VIEW_ID],[AVR_CHECK_NAME],[AVR_TABLE_NAME],[AVR_COL_SHOW_NAME],[AVR_COL_NAME]) values(@AVR_VIEW_ID,@AVR_CHECK_NAME,@AVR_TABLE_NAME,@AVR_COL_SHOW_NAME,@AVR_COL_NAME);";

                command.Parameters.Add(new OleDbParameter("AVR_VIEW_ID", avr_view_id));
                command.Parameters.Add(new OleDbParameter("AVR_CHECK_NAME", avr_check_name));
                command.Parameters.Add(new OleDbParameter("AVR_TABLE_NAME", avr_table_name));
                command.Parameters.Add(new OleDbParameter("AVR_COL_SHOW_NAME", avr_col_show_name));
                command.Parameters.Add(new OleDbParameter("AVR_COL_NAME", avr_col_name));

                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Update_T_VIEW_CONFIG(string avr_view_id, string avr_check_name, string avr_table_name, string avr_col_show_name, string avr_col_name)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"update T_VIEW_CONFIG set [AVR_VIEW_ID]=@AVR_VIEW_ID, [AVR_CHECK_NAME]=@AVR_CHECK_NAME, [AVR_TABLE_NAME]=@AVR_TABLE_NAME, [AVR_COL_SHOW_NAME]=@AVR_COL_SHOW_NAME, [AVR_COL_NAME]=@AVR_COL_NAME where [AVR_VIEW_ID]='{avr_view_id}' and [AVR_CHECK_NAME]='{avr_check_name}';";

                command.Parameters.Add(new OleDbParameter("AVR_VIEW_ID", avr_view_id));
                command.Parameters.Add(new OleDbParameter("AVR_CHECK_NAME", avr_check_name));
                command.Parameters.Add(new OleDbParameter("AVR_TABLE_NAME", avr_table_name));
                command.Parameters.Add(new OleDbParameter("AVR_COL_SHOW_NAME", avr_col_show_name));
                command.Parameters.Add(new OleDbParameter("AVR_COL_NAME", avr_col_name));

                int row = command.ExecuteNonQuery();
                ShowMsg($"更新{row}行");
            }
        }

        private void Insert_T_USER_INFO(string user_id, string user_name, string user_password, string user_power)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_USER_INFO([USER_ID], [USER_NAME], [USER_PASSWORD], [USER_POWER]) values(@USER_ID,@USER_NAME,@USER_PASSWORD,@USER_POWER);";

                command.Parameters.Add(new OleDbParameter("USER_ID", user_id));
                command.Parameters.Add(new OleDbParameter("USER_NAME", user_name));
                command.Parameters.Add(new OleDbParameter("USER_PASSWORD", user_password));
                command.Parameters.Add(new OleDbParameter("USER_POWER", user_power));

                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Insert_T_MENU_VIEW(string menu_name, string menu_class, string menu_image, string valid_flag, string menu_datasource, string menu_check_enable, string menu_user_visible, string menu_main, string menu_category, string sort_id)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"insert into T_MENU_VIEW([MENU_NAME],[MENU_CLASS],[MENU_IMAGE],[VALID_FLAG],[MENU_DATASOURCE],[MENU_CHECK_ENABLE],[MENU_USER_VISIBLE],[MENU_MAIN],[MENU_CATEGORY],[SORT_ID]) values(@MENU_NAME,@MENU_CLASS,@MENU_IMAGE,@VALID_FLAG,@MENU_DATASOURCE,@MENU_CHECK_ENABLE,@MENU_USER_VISIBLE,@MENU_MAIN,@MENU_CATEGORY,@SORT_ID);";

                command.Parameters.Add(new OleDbParameter("MENU_NAME", menu_name));
                command.Parameters.Add(new OleDbParameter("MENU_CLASS", menu_class));
                command.Parameters.Add(new OleDbParameter("MENU_IMAGE", menu_image));
                command.Parameters.Add(new OleDbParameter("VALID_FLAG", valid_flag));
                command.Parameters.Add(new OleDbParameter("MENU_DATASOURCE", menu_datasource));
                command.Parameters.Add(new OleDbParameter("MENU_CHECK_ENABLE", menu_check_enable));
                command.Parameters.Add(new OleDbParameter("MENU_USER_VISIBLE", menu_user_visible));
                command.Parameters.Add(new OleDbParameter("MENU_MAIN", menu_main));
                command.Parameters.Add(new OleDbParameter("MENU_CATEGORY", menu_category));
                command.Parameters.Add(new OleDbParameter("SORT_ID", sort_id));

                ShowMsg($"新增{command.ExecuteNonQuery()}行");
            }
        }

        private void Update_T_MENU_VIEW(string menu_name, string menu_class, string menu_image, string valid_flag, string menu_datasource, string menu_check_enable, string menu_user_visible, string menu_main, string menu_category, string sort_id)
        {
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();
                OleDbCommand command = connection.CreateCommand();
                command.CommandText = $"update T_MENU_VIEW set [MENU_CLASS]=@MENU_CLASS, [MENU_IMAGE]=@MENU_IMAGE, [VALID_FLAG]=@VALID_FLAG, [MENU_DATASOURCE]=@MENU_DATASOURCE, [MENU_CHECK_ENABLE]=@MENU_CHECK_ENABLE, [MENU_USER_VISIBLE]=@MENU_USER_VISIBLE, [MENU_MAIN]=@MENU_MAIN, [MENU_CATEGORY]=@MENU_CATEGORY, [SORT_ID]=@SORT_ID where [MENU_NAME]='{menu_name}';";

                command.Parameters.Add(new OleDbParameter("MENU_NAME", menu_name));
                command.Parameters.Add(new OleDbParameter("MENU_CLASS", menu_class));
                command.Parameters.Add(new OleDbParameter("MENU_IMAGE", menu_image));
                command.Parameters.Add(new OleDbParameter("VALID_FLAG", valid_flag));
                command.Parameters.Add(new OleDbParameter("MENU_DATASOURCE", menu_datasource));
                command.Parameters.Add(new OleDbParameter("MENU_CHECK_ENABLE", menu_check_enable));
                command.Parameters.Add(new OleDbParameter("MENU_USER_VISIBLE", menu_user_visible));
                command.Parameters.Add(new OleDbParameter("MENU_MAIN", menu_main));
                command.Parameters.Add(new OleDbParameter("MENU_CATEGORY", menu_category));
                command.Parameters.Add(new OleDbParameter("SORT_ID", sort_id));

                int row = command.ExecuteNonQuery();
                ShowMsg($"更新{row}行");
            }
        }

        #endregion CURD
    }
}
