using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Koenig.Maestro.Operation.Data
{
    public class Database : IDisposable
    {
        static readonly DateTime minimumDBDate = new DateTime(1753, 1, 1);

        static readonly Logger logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        static readonly IsolationLevel defaultIsolationLevel = IsolationLevel.ReadCommitted;

        List<SqlReader> readers = new List<SqlReader>();
        SqlConnection connection;
        SqlTransaction transaction;
        int commandTimeout = 30;

        public Database()
        {



        }

        public bool InTransaction
        {
            get { return (transaction != null); }
        }

        protected virtual void SetConnection()
        {

            if (connection == null)
                connection = new SqlConnection(MaestroApplication.Instance.ConnectionString);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

        }


        protected virtual SqlCommand CreateCommand(SpCall sp, int timeout)
        {
            SqlCommand cmd = new SqlCommand(sp.ProcName, connection);
            cmd.CommandType = sp.CommandType;
            cmd.CommandTimeout = timeout;
            if (InTransaction)
            {
                cmd.Transaction = transaction;
            }

            foreach (SqlParameter parameter in sp.Parameters.Values)
            {
                if (parameter.DbType == DbType.Date || parameter.DbType == DbType.DateTime || parameter.DbType == DbType.DateTime2)
                {
                    object o = parameter.Value;
                    if (o != null && Type.GetTypeCode(o.GetType()) == TypeCode.DateTime)
                    {
                        if (minimumDBDate > (DateTime)o)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }
                }
                /*
                if ((parameter.SqlDbType == SqlDbType.Char || parameter.SqlDbType == SqlDbType.NChar || parameter.SqlDbType == SqlDbType.NVarChar || parameter.SqlDbType == SqlDbType.VarChar)
                    && privityService.IsPrivateInformation(parameter.ParameterName))
                {
                    var parameterValue = privityService.Encrypt(parameter.Value.ToString());
                    parameter.Value = parameterValue;
                    if (parameterValue != null && parameter.Size < parameterValue.Length)
                    {
                        parameter.Size = parameterValue.Length;
                    }
                }*/
                cmd.Parameters.Add(parameter);
            }
            return cmd;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CloseReaders();
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex, "Can't close connection");
                    }
                }
                connection.Dispose();
            }
        }

        protected void CloseReaders()
        {
            if (readers == null)
            {
                return;
            }
            foreach (SqlReader reader in readers)
            {
                if (!reader.ContainedReader.IsClosed)
                {
                    //Debug mode kullanılarak (web.config içinde) çalıştırılan tüm SP'ler loglanabilir.
                    // Bu sayede kapatılmayan SqlDataReader'ı bulmak daha kolay olur.
                    logger.Warn("Kapatılmayan SqlDataReader kapatıldı.");
                }
            }
        }


        protected void LogCommandInfo(string method, SpCall sp)
        {
            //if (!Configuration.ConfigManager.LogDbCommands)
                //return;

            List<string> args = new List<string>();
            foreach (SqlParameter parameter in sp.Parameters.Values)
            {
                if (parameter.Value == null || parameter.Value == DBNull.Value)
                {
                    args.Add(string.Format("{0}=NULL", parameter.ParameterName));
                }
                else
                {
                    switch (parameter.SqlDbType)
                    {
                        case SqlDbType.Char:
                        case SqlDbType.NChar:
                        case SqlDbType.NText:
                        case SqlDbType.NVarChar:
                        case SqlDbType.VarChar:
                        case SqlDbType.Timestamp:
                        case SqlDbType.Text:
                        case SqlDbType.Xml:
                            args.Add(string.Format("{0}='{1}'", parameter.ParameterName, parameter.Value));
                            break;

                        case SqlDbType.DateTime:
                        case SqlDbType.SmallDateTime:
                            DateTime dt = (DateTime)parameter.Value;
                            args.Add(string.Format("{0}='{1}'", parameter.ParameterName, dt.ToString("yyyyMMdd")));
                            break;

                        default:
                            args.Add(string.Format("{0}={1}", parameter.ParameterName, parameter.Value));
                            break;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} : {1} ", method, sp.ProcName);
            sb.Append(string.Join(", ", args.ToArray()));
            logger.Debug(sb.ToString());
        }

        public void CloseDbConnection()
        {
            if (InTransaction)
            {
                throw new InvalidOperationException("Open transaction found, commit/rollback transaction first.");
            }
            else
            {
                try
                {
                    connection.Close();
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex);
                    throw;
                }
            }
        }

        public void BeginTransaction()
        {
            SetConnection();
            transaction = connection.BeginTransaction(defaultIsolationLevel);
        }

        public void CommitTransaction()
        {
            if (!InTransaction)
            {
                logger.Fatal("No open transaction found to Commit.");
                throw new Exception("No open transaction found to Commit.");
            }
            else
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
            connection.Close();
        }

        public void RollbackTransaction()
        {
            if (InTransaction)
            {
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;

                connection.Close();
            }
        }

        protected virtual DataTable CreateDataTable(SpCall spCall)
        {
            using (SqlCommand command = CreateCommand(spCall))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                    }

                    while (reader.Read())
                    {
                        object[] values = new object[fieldCount];
                        reader.GetValues(values);
                        dt.Rows.Add(values);
                    }
                    return dt;
                }

            }
        }

        SqlCommand CreateCommand(SpCall spCall)
        {
            SqlCommand cmd = new SqlCommand(spCall.ProcName, connection);
            cmd.CommandType = spCall.CommandType;
            cmd.CommandTimeout = commandTimeout;
            if (InTransaction)
            {
                cmd.Transaction = transaction;
            }

            foreach (SqlParameter parameter in spCall.Parameters.Values)
            {
                if (parameter.DbType == DbType.Date || parameter.DbType == DbType.DateTime || parameter.DbType == DbType.DateTime2)
                {
                    object o = parameter.Value;
                    if (o != null && Type.GetTypeCode(o.GetType()) == TypeCode.DateTime)
                    {
                        if (minimumDBDate > (DateTime)o)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }
                }
                cmd.Parameters.Add(parameter);
            }
            return cmd;
        }

        public SqlReader ExecuteReader(SpCall spCall)
        {
            if (logger.IsDebugEnabled)
            {
                LogCommandInfo("Select", spCall);
            }

            SetConnection();
            SqlCommand cmd = null;

            try
            {
                cmd = CreateCommand(spCall);
                SqlReader reader = new SqlReader(cmd, spCall);
                readers.Add(reader);
                return reader;
            }
            catch (SqlException e)
            {
                logger.Fatal(e, string.Format("Exception while executing spCall{0}{1}", Environment.NewLine, spCall.ToString()));
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public T ExecuteScalar<T>(SpCall spCall)
        {
            if (logger.IsDebugEnabled)
                LogCommandInfo("ExecuteScalar", spCall);

            SetConnection();

            using (SqlCommand cmd = CreateCommand(spCall))
            {
                DateTime start = DateTime.Now;
                try
                {
                    object result = cmd.ExecuteScalar();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                catch (SqlException e)
                {
                    logger.Fatal(e, string.Format("Exception while executing spCall{0}{1}", Environment.NewLine, spCall.ToString()));
                    throw;
                }
                finally
                {
                    TimeSpan span = DateTime.Now - start;

                    if (!InTransaction)
                    {
                        CloseDbConnection();
                    }
                    cmd.Parameters.Clear();
                }
            }
        }

        public DataTable ExecuteDataTable(SpCall spCall)
        {
            if (logger.IsDebugEnabled)
                LogCommandInfo("ExecuteDataTable", spCall);

            SetConnection();
            DataTable dt = CreateDataTable(spCall);
            if (!InTransaction)
            {
                CloseDbConnection();
            }
            return dt;
        }

        public DataSet ExecuteDataSet(SpCall spCall)
        {
            if (logger.IsDebugEnabled)
                LogCommandInfo("ExecuteDataSet", spCall);

            SetConnection();
            using (SqlCommand cmd = CreateCommand(spCall))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DateTime start = DateTime.Now;
                    try
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                    catch (SqlException e)
                    {
                        logger.Fatal(e, string.Format("Exception while executing spCall{0}{1}", Environment.NewLine, spCall.ToString()));
                        throw;
                    }
                    finally
                    {
                        TimeSpan span = DateTime.Now - start;

                        if (!InTransaction)
                        {
                            CloseDbConnection();
                        }
                        cmd.Parameters.Clear();
                    }
                }
            }
        }

        public int ExecuteNonQuery(SpCall spCall)
        {
            SetConnection();
            SqlCommand command = CreateCommand(spCall);

            if (logger.IsDebugEnabled)
                LogCommandInfo("ExecuteNonQuery", spCall);

            if (command == null)
                throw new ArgumentNullException("command");

            try
            {
                DateTime start = DateTime.Now;
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    logger.Fatal(e, string.Format("Exception while executing spCall{0}{1}", Environment.NewLine, spCall.ToString()));
                    throw;
                }
                finally
                {
                    TimeSpan span = DateTime.Now - start;

                    if (!InTransaction)
                    {
                        CloseDbConnection();
                    }
                    command.Parameters.Clear();

                    /*
                    if (span.TotalMilliseconds > thresholdTimeValue)
                    {
                        LogLongRunningCall(span, sp);
                    }*/
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        public void ExecuteBulkInsert(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
                ExecuteBulkInsert(dt);

        }

        public void ExecuteBulkInsert(DataTable dt)
        {
            SetConnection();
            SqlBulkCopy bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulk.DestinationTableName = dt.TableName;
            bulk.WriteToServer(dt);
        }
    }
}
