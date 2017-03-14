using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace WeChat.NET.Helper
{
    /// <summary>
    /// 适应多种数据库操作工具类
    /// </summary>
    public class DBHelper
    {
        #region 变量
        private DbConnection _Connection;
        private string _ProviderName;
        private string _ConnectionString;
        private string _DataBaseName;
        #endregion

        #region 属性
        /// <summary>
        /// 库名
        /// </summary>
        public string DataBaseName
        {
            get
            {
                return _DataBaseName;
            }
        }
        #endregion

        #region 构造函数
        public DBHelper()
            : this("DeafultDB")
        {
        }

        /// <summary>
        /// 通过连接字符串的名称进行构造
        /// </summary>
        /// <param name="connName">数据库连接字符串的名称</param>
        public DBHelper(string connName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connName];
            if (settings == null)
                throw new ConfigurationException(string.Format(
                    "找不到指定数据库连接名称({0})的配置信息", connName));

            this._ProviderName = settings.ProviderName;
            this._ConnectionString = settings.ConnectionString;
            this._Connection = CreateConnection(_ConnectionString, _ProviderName);
        }

        public DBHelper(string connectionString, string providerName)
        {
            this._ProviderName = providerName;
            this._ConnectionString = connectionString;
            this._Connection = CreateConnection(_ConnectionString, _ProviderName);
        }
        #endregion

        #region 创建新连接
        public DbConnection CreateConnection(string connectionString)
        {
            return CreateConnection(connectionString, "System.Data.SqlClient");
        }

        public DbConnection CreateConnection(string connectionString, string providerName)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(providerName);

            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = connectionString;
            this._DataBaseName = dbconn.Database;
            return dbconn;
        }
        #endregion

        #region 增加参数
        /// <summary>
        /// 增加参数集合
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="dbParameterCollection"></param>
        private void AddParameterCollection(DbCommand cmd, DbParameter[] dbParameterCollection)
        {
            if (dbParameterCollection != null)
            {
                foreach (DbParameter dbParameter in dbParameterCollection)
                {
                    cmd.Parameters.Add(dbParameter);
                }
            }
        }
        #endregion

        #region SQL无返回值可操作的方法

        /// <summary>
        /// 带错误输出，无返回值的SQL执行（默认是Text）
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <returns></returns>
        public string ExecuteNonQuery(string sqlQuery)
        {
            return ExecuteNonQuery(sqlQuery, CommandType.Text);
        }

        /// <summary>
        /// 带错误输出，无返回值的SQL执行（默认是Text）
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public string ExecuteNonQuery(string sqlQuery, int timeOut)
        {
            return ExecuteNonQuery(sqlQuery, CommandType.Text, timeOut);
        }

        /// <summary>
        /// 带SQL执行类型，无返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public string ExecuteNonQuery(string sqlQuery, CommandType cmdtype)
        {
            return ExecuteNonQuery(sqlQuery, null, cmdtype);
        }

        /// <summary>
        /// 带SQL执行类型，无返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public string ExecuteNonQuery(string sqlQuery, CommandType cmdtype, int timeOut)
        {
            return ExecuteNonQuery(sqlQuery, null, cmdtype, timeOut);
        }

        /// <summary>
        /// 带参，带SQL执行类型，无返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public string ExecuteNonQuery(string sqlQuery, DbParameter[] parms, CommandType cmdtype)
        {
            return ExecuteNonQuery(sqlQuery, parms, cmdtype, 30);
        }

        /// <summary>
        /// 带参，带SQL执行类型，无返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public string ExecuteNonQuery(string sqlQuery, DbParameter[] parms, CommandType cmdtype, int timeOut)
        {
            string retVal = "0";

            // 创建DbCommand
            DbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = sqlQuery;
            cmd.CommandType = cmdtype;
            cmd.CommandTimeout = timeOut;

            // 增加参数
            AddParameterCollection(cmd, parms);

            try
            {
                if (_Connection.State == ConnectionState.Closed)
                {
                    _Connection.Open();
                }
                retVal = cmd.ExecuteNonQuery().ToString();
            }
            catch (Exception exp)
            {
                // retVal = exp.Message;
                throw exp;
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }
            return retVal;
        }

        #endregion

        #region SQL有返回值可操作的方法

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlQuery)
        {
            return ExecuteScalar(sqlQuery, CommandType.Text);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlQuery, int timeOut)
        {
            return ExecuteScalar(sqlQuery, CommandType.Text, timeOut);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlQuery, CommandType cmdtype)
        {
            return ExecuteScalar(sqlQuery, null, cmdtype);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlQuery, CommandType cmdtype, int timeOut)
        {
            return ExecuteScalar(sqlQuery, null, cmdtype, timeOut);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlQuery, DbParameter[] parms, CommandType cmdtype)
        {
            return ExecuteScalar(sqlQuery, parms, cmdtype, 30);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlQuery, DbParameter[] parms, CommandType cmdtype, int timeOut)
        {
            object retVal = null;

            // 创建DbCommand
            DbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = sqlQuery;
            cmd.CommandType = cmdtype;
            cmd.CommandTimeout = timeOut;

            // 增加参数
            AddParameterCollection(cmd, parms);

            try
            {
                if (_Connection.State == ConnectionState.Closed)
                {
                    _Connection.Open();
                }
                retVal = cmd.ExecuteScalar();
            }
            catch (Exception exp)
            {
                //retVal = (object)exp.Message;
                throw exp;
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }

            return retVal;
        }

        #endregion

        #region 返回数据读取器(DataReader)可操作的方法

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlQuery)
        {
            return ExecuteReader(sqlQuery, CommandType.Text);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlQuery, int timeOut)
        {
            return ExecuteReader(sqlQuery, CommandType.Text, timeOut);
        }

        /// <summary>
        /// 带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlQuery, CommandType cmdtype)
        {
            return ExecuteReader(sqlQuery, null, cmdtype);
        }

        /// <summary>
        /// 带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlQuery, CommandType cmdtype, int timeOut)
        {
            return ExecuteReader(sqlQuery, null, cmdtype, timeOut);
        }

        /// <summary>
        /// 带参，带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlQuery, DbParameter[] parms, CommandType cmdtype)
        {
            return ExecuteReader(sqlQuery, parms, cmdtype, 30);
        }

        /// <summary>
        /// 带参，带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlQuery, DbParameter[] parms, CommandType cmdtype, int timeOut)
        {
            DbDataReader reader;

            // 创建DbCommand
            DbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = sqlQuery;
            cmd.CommandType = cmdtype;
            cmd.CommandTimeout = timeOut;

            // 增加参数
            AddParameterCollection(cmd, parms);

            try
            {
                if (_Connection.State == ConnectionState.Closed)
                {
                    _Connection.Open();
                }
                //将关闭数据库的操作交给Read操作，即关闭了read就关闭了数据库
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        #endregion

        #region 返回数据表(DataTable)可操作的方法

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sqlQuery)
        {
            return ExecuteTable(sqlQuery, CommandType.Text);
        }

        /// <summary>
        /// 有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sqlQuery, int timeOut)
        {
            return ExecuteTable(sqlQuery, CommandType.Text, timeOut);
        }

        /// <summary>
        /// 带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sqlQuery, CommandType cmdtype)
        {
            return ExecuteTable(sqlQuery, null, cmdtype);
        }

        /// <summary>
        /// 带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sqlQuery, CommandType cmdtype, int timeOut)
        {
            return ExecuteTable(sqlQuery, null, cmdtype, timeOut);
        }

        /// <summary>
        /// 带参，带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sqlQuery, DbParameter[] parms, CommandType cmdtype)
        {
            return ExecuteTable(sqlQuery, parms, cmdtype, 60);
        }

        /// <summary>
        /// 带参，带SQL执行类型，有返回值的SQL执行
        /// </summary>
        /// <param name="sqlQuery">sql语句</param>
        /// <param name="parms">参数</param>
        /// <param name="cmdtype">命令类型</param>
        /// <param name="timeOut">延迟等待时间</param>
        /// <returns></returns>
        public DataTable ExecuteTable(string sqlQuery, DbParameter[] parms, CommandType cmdtype, int timeOut)
        {
            // 创建DbCommand
            DbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = sqlQuery;
            cmd.CommandType = cmdtype;
            //cmd.CommandTimeout = timeOut;

            // 增加参数
            AddParameterCollection(cmd, parms);

            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(_ProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();


            try
            {
                if (_Connection.State == ConnectionState.Closed)
                {
                    _Connection.Open();
                }
                //将关闭数据库的操作交给Read操作，即关闭了read就关闭了数据库
                dbDataAdapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }
        }

        #endregion

        #region 通用数据库的新增、修改、删除等操作
        /// <summary>
        /// 判断是否存在满足条件的记录
        /// </summary>
        /// <param name="filterExpression">过滤表达式</param>
        /// <returns>bool</returns>
        public bool HasRecord(string tableName, string filterExpression)
        {
            string sql = string.Format("select count(*) from [{0}](nolock) where 1=1 AND {1}", tableName, filterExpression);

            object obj = ExecuteScalar(sql);

            return Convert.ToInt32(obj) > 0;
        }

        /// <summary>
        /// 插入表数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="data">表数据</param>
        public void InsertTable(string tableName, IDictionary<string, string> data)
        {
            // 组装插入Sql语句
            string strFields = string.Empty;
            string strValues = string.Empty;
            foreach (var pair in data)
            {
                strFields += string.Format(",[{0}]", pair.Key);
                strValues += string.Format(",'{0}'",
                    string.IsNullOrEmpty(pair.Value) ? "" : pair.Value.Replace("'", "''"));
            }
            string insertSql = string.Format("insert [{0}]({1}) values({2})",
                tableName, strFields.TrimStart(','), strValues.TrimStart(','));

            // 执行插入表单数据
            string result = ExecuteNonQuery(insertSql);
            if (result != "1")
                throw new Exception(string.Format("向（{0}）表插入数据出错,错误信息：{0}", result));
        }

        /// <summary>
        /// 更新表数据
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="data">表数据</param>
        /// <param name="filterExpression">过滤表达式</param>
        public void UpdateTable(string tableName, IDictionary<string, string> data, string filterExpression)
        {
            // 组装更新Sql语句
            string sql = string.Empty;
            foreach (var pair in data)
            {
                sql += string.Format(",[{0}]='{1}'", pair.Key,
                    string.IsNullOrEmpty(pair.Value) ? "" : pair.Value.Replace("'", "''"));
            }
            string updateSql = string.Format("update [{0}] set {1} where 1=1 AND {2}",
                tableName, sql.TrimStart(','), filterExpression);

            // 执行更新表单数据
            string result = ExecuteNonQuery(updateSql);
            if (result != "1")
                throw new Exception(string.Format("更新（{0}）表数据时出错,错误信息：{1}", tableName, result));
        }

        /// <summary>
        /// 保存表数据（如果存在，则更新数据，否则新增数据）
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="data">表数据</param>
        /// <param name="filterExpression">过滤表达式</param>
        public void SaveTable(string tableName, IDictionary<string, string> data, string filterExpression)
        {
            if (HasRecord(tableName, filterExpression))
            {
                UpdateTable(tableName, data, filterExpression);
            }
            else
            {
                InsertTable(tableName, data);
            }
        }
        #endregion
    }

    /// <summary>
    /// DbDataReader的扩展方法
    /// </summary>
    public static class DbDataReaderExtension
    {
        public static T Get<T>(this DbDataReader reader, string fieldName)
        {
            return Get<T>(reader, fieldName, default(T));
        }

        public static T Get<T>(this DbDataReader reader, string fieldName, T defaultValue)
        {
            if (reader == null || string.IsNullOrEmpty(fieldName))
                return defaultValue;

            try
            {
                object o = reader[fieldName];
                if (o == null || o == DBNull.Value)
                    return defaultValue;
                else
                {
                    if (typeof(T) == typeof(int))
                    {
                        o = int.Parse(o.ToString());
                    }
                    else if (typeof(T) == typeof(decimal))
                    {
                        o = decimal.Parse(o.ToString());
                    }
                    else if (typeof(T) == typeof(DateTime))
                    {
                        o = DateTime.Parse(o.ToString());
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        o = double.Parse(o.ToString());
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        o = float.Parse(o.ToString());
                    }
                    return (T)o;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
