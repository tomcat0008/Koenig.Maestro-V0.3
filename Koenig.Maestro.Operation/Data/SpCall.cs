using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Koenig.Maestro.Operation.Data
{
    public class SpCall
    {
        public SpCall()
        {

        }

        public SpCall(string procName)
        {
            ProcName = procName;
            CommandType = CommandType.StoredProcedure;
        }

        public SpCall(string procName, CommandType commandType)
        {
            ProcName = procName;
            CommandType = commandType;
        }

        public string ProcName { get; set; }

        public CommandType CommandType { get; set; }

        public Dictionary<string, SqlParameter> Parameters { get; } = new Dictionary<string, SqlParameter>();

        public void SetStructured<T>(string parameterName, string typeName, string columnName, List<T> value)
        {
            SqlParameter par = new SqlParameter(parameterName, SqlDbType.Structured);
            par.TypeName = typeName;

            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn(columnName, typeof(T)));
            value.ForEach(v => tbl.Rows.Add(v));

            par.Value = tbl;

            par.Direction = ParameterDirection.Input;

            if (Parameters.ContainsKey(parameterName))
                Parameters[parameterName] = par;
            else
                Parameters.Add(parameterName, par);
        }

        public void SetVarchar(string parameterName, string value)
        {
            SetParameter(parameterName, SqlDbType.VarChar, ParameterDirection.Input, value);
        }

        public void SetVarchar(string parameterName, ParameterDirection direction, string value)
        {
            SetParameter(parameterName, SqlDbType.VarChar, direction, value);
        }

        public void SetBigInt(string parameterName, long value)
        {
            SetParameter(parameterName, SqlDbType.BigInt, ParameterDirection.Input, value);
        }

        public void SetBigInt(string parameterName, ParameterDirection direction, long value)
        {
            SetParameter(parameterName, SqlDbType.BigInt, direction, value);
        }

        public void SetInt(string parameterName, int value)
        {
            SetParameter(parameterName, SqlDbType.Int, ParameterDirection.Input, value);
        }

        public void SetInt(string parameterName, ParameterDirection direction, int value)
        {
            SetParameter(parameterName, SqlDbType.Int, direction, value);
        }

        public void SetBinary(string parameterName, byte[] value)
        {
            SetParameter(parameterName, SqlDbType.Binary, ParameterDirection.Input, value);
        }

        public void SetBinary(string parameterName, ParameterDirection direction, byte[] value)
        {
            SetParameter(parameterName, SqlDbType.Binary, direction, value);
        }

        public void SetBit(string parameterName, bool value)
        {
            SetParameter(parameterName, SqlDbType.Bit, ParameterDirection.Input, value);
        }

        public void SetBit(string parameterName, ParameterDirection direction, bool value)
        {
            SetParameter(parameterName, SqlDbType.Bit, direction, value);
        }

        public void SetDateTime(string parameterName, DateTime value)
        {
            SetParameter(parameterName, SqlDbType.DateTime, ParameterDirection.Input, value);
        }

        public void SetDateTime(string parameterName, ParameterDirection direction, DateTime value)
        {
            SetParameter(parameterName, SqlDbType.DateTime, direction, value);
        }

        public void SetDecimal(string parameterName, decimal value)
        {
            SetParameter(parameterName, SqlDbType.Decimal, ParameterDirection.Input, value);
        }

        public void SetDecimal(string parameterName, ParameterDirection direction, decimal value)
        {
            SetParameter(parameterName, SqlDbType.Decimal, direction, value);
        }

        public void SetSmallInt(string parameterName, short value)
        {
            SetParameter(parameterName, SqlDbType.SmallInt, ParameterDirection.Input, value);
        }

        public void SetSmallInt(string parameterName, ParameterDirection direction, short value)
        {
            SetParameter(parameterName, SqlDbType.SmallInt, direction, value);
        }

        public void SetTinyInt(string parameterName, byte value)
        {
            SetParameter(parameterName, SqlDbType.TinyInt, ParameterDirection.Input, value);
        }

        public void SetTinyInt(string parameterName, ParameterDirection direction, byte value)
        {
            SetParameter(parameterName, SqlDbType.TinyInt, direction, value);
        }

        void SetParameter(string parameterName, SqlDbType sqlDbType, ParameterDirection direction, object value)
        {
            SqlParameter par = new SqlParameter(parameterName, sqlDbType);
            par.Value = value ?? DBNull.Value;

            par.Direction = direction;

            if (Parameters.ContainsKey(parameterName))
                Parameters[parameterName] = par;
            else
                Parameters.Add(parameterName, par);

        }

        public SqlParameter this[string parameterName]
        {
            get
            {
                SqlParameter result = null;

                if (Parameters.ContainsKey(parameterName))
                    result = Parameters[parameterName];
                return result;
            }

        }


    }
}
