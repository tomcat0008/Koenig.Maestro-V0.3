using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Koenig.Maestro.Operation.Data
{
    public class SqlReader : IDisposable
    {
        static readonly Logger logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        int rowIndex;
        readonly SpCall sp;
        public SqlDataReader ContainedReader { get; private set; }

        internal SqlReader(SqlDataReader reader, SpCall sp)
        {
            ContainedReader = reader;
            this.sp = sp;
            rowIndex = 0;
        }

        internal SqlReader(SqlCommand command, SpCall sp)
        {
            this.sp = sp;
            ContainedReader = command.ExecuteReader();
            rowIndex = 0;
        }

        public bool HasRows
        {
            get { return ContainedReader.HasRows; }
        }

        public virtual void Dispose()
        {
            ContainedReader.Dispose();
        }

        public void Close()
        {
            if (ContainedReader != null)
            {
                ContainedReader.Close();
            }
        }

        public bool IsDBNull(string colName)
        {
            return ContainedReader.IsDBNull(GetOrdinal(colName));
        }

        int GetOrdinal(string columnName)
        {
            try
            {
                return ContainedReader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException e)
            {
                string msg = string.Format("Column {0} does not exist in the resultset of SP {1}", columnName, sp.ProcName);
                logger.Fatal(e, msg);

                throw;
            }


        }

        public string GetName(int i)
        {
            try
            {
                return ContainedReader.GetName(i);
            }
            catch (IndexOutOfRangeException e)
            {

                string msg = string.Format("Column index {0} does not exist in the resultset of SP {1}", i, sp.ProcName);
                logger.Fatal(e, msg);
                throw;
            }
        }

        public object GetObject(int ordinal)
        {
            return ContainedReader[ordinal];
        }
        public virtual object GetObject(string colName)
        {
            return ContainedReader[colName];
        }

        public virtual string GetString(string colName)
        {
            return ContainedReader.IsDBNull(GetOrdinal(colName)) ? null : ContainedReader[colName].ToString().Trim();
        }

        public virtual string GetString(int index)
        {
            return ContainedReader.IsDBNull(index) ? null : ContainedReader[index].ToString().Trim();
        }

        public virtual bool GetBool(string colName)
        {
            if (ContainedReader.IsDBNull(GetOrdinal(colName)))
            {
                return false;
            }
            return bool.Parse(ContainedReader[colName].ToString());
        }

        public virtual long GetInt64(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column)) ? 0 : ((IConvertible)ContainedReader[column]).ToInt64(null);
        }

        public virtual Guid GetGuid(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column)) ? Guid.Empty : (Guid)ContainedReader[column];
        }

        public virtual int GetInt32(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column))
                       ? 0
                       : ((IConvertible)ContainedReader[column]).ToInt32(null);
        }

        public virtual short GetInt16(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column)) ? (short)0 : short.Parse(ContainedReader[column].ToString());
        }

        public float GetFloat(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column)) ? 0 : float.Parse(ContainedReader[column].ToString());
        }

        public virtual double GetDouble(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column))
                       ? 0
                       : double.Parse(ContainedReader[column].ToString());
        }

        public decimal GetDecimal(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column))
                       ? 0
                       : decimal.Parse(ContainedReader[column].ToString());
        }

        public virtual DateTime GetDateTime(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column)) ? DateTime.MinValue : DateTime.Parse(ContainedReader[column].ToString());
        }

        public Enum GetEnum(string column, Type enumType)
        {
            return (Enum)Enum.Parse(enumType, GetString(column), true);
        }

        public char GetChar(string column)
        {
            return ContainedReader.IsDBNull(GetOrdinal(column)) ? char.MinValue : ContainedReader[column].ToString()[0];
        }

        public virtual bool Read()
        {
            rowIndex++;
            return ContainedReader.Read();
        }

        public bool NextResult()
        {
            return ContainedReader.NextResult();
        }

        public int FieldCount
        {
            get
            {
                return ContainedReader.FieldCount;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("RowIndex=");
            sb.Append(rowIndex).Append("  ");

            for (int i = 0; i < ContainedReader.FieldCount; i++)
            {
                sb.Append(ContainedReader.GetName(i));
                sb.AppendFormat("{0}=|{1}",
                    ContainedReader.GetName(i),
                    Utility.StringUtils.ToString(ContainedReader.GetProviderSpecificValue(i)));

                if (i < ContainedReader.FieldCount - 1)
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }

        public virtual T GetValue<T>(string colName)
        {
            return GetValue<T>(colName, default(T));
        }

        public virtual T GetValue<T>(string colName, T defaultValue)
        {
            if (ContainedReader.IsDBNull(GetOrdinal(colName)))
            {
                return defaultValue;
            }
            Type t = typeof(T);
            if (t == typeof(string))
            {
                return (T)Convert.ChangeType((string)ContainedReader[colName], t);
            }
            t = Nullable.GetUnderlyingType(t) ?? t;

            if (t.IsEnum)
            {
                return (T)Enum.Parse(t, (string)ContainedReader[colName]);
            }
            else
            {
                return (T)Convert.ChangeType(ContainedReader[colName], t);
            }
        }

    }

}
