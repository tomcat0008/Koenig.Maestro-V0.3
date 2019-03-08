using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.DbEntities;
using Koenig.Maestro.Entity.Query;
using Koenig.Maestro.Operation.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal sealed class TranLogManager : ManagerBase
    {
        public TranLogManager(TransactionContext context) : base(context) { }

        public void InsertTransactionLog(TransactionLog tranLog)
        {
            SpCall call = new SpCall("COR.TRANSACTION_LOG_INSERT");
            call.SetVarchar("@TRAN_CODE", tranLog.TransactionCode);
            call.SetBinary("@TRANSACTION_LOG", tranLog.Log);
            call.SetVarchar("@TRANSACTION_JSON", tranLog.LogJson);
            call.SetVarchar("@TRANSACTION_STATUS", tranLog.Status);
            call.SetBigInt("@TRANSACTION_OBJECT_ID", tranLog.LogObjectId);
            call.SetVarchar("@ACTION_TYPE", tranLog.ActionType);
            call.SetVarchar("@REQUEST_TYPE", tranLog.RequestType);
            call.SetDateTime("@CREATE_DATE", DateTime.Now);
            call.SetVarchar("@CREATE_USER", tranLog.CreatedUser);
            call.SetDecimal("@DURATION", tranLog.Duration);
            long id = db.ExecuteScalar<long>(call);
            tranLog.Id = id;
        }

        public List<TransactionLog> List(TransactionLogQuery logQuery)
        {
            List<TransactionLog> result = new List<TransactionLog>();

            SpCall call = new SpCall("COR.TRANSACTION_LOG_LIST");
            call.SetBigInt("@ID", logQuery.Id);
            call.SetVarchar("@TRAN_CODE", logQuery.TransactionCode);
            call.SetVarchar("@TRANSACTION_STATUS", logQuery.Status);
            call.SetDateTime("@BEGIN_DATE", logQuery.BeginDate);
            call.SetDateTime("@END_DATE", logQuery.EndDate);
            call.SetVarchar("@CREATE_USER", logQuery.User);

            using (SqlReader reader = db.ExecuteReader(call))
            {
                while(reader.Read())
                {
                    TransactionLog log = new TransactionLog()
                    {
                        Id = reader.GetInt64("ID"),
                        CreateDate = reader.GetDateTime("CREATE_DATE"),
                        CreatedUser = reader.GetString("CREATE_USER"),
                        Log = reader.GetValue<byte[]>("TRANSACTION_LOG"),
                        LogJson = reader.GetString("TRANSACTION_JSON"),
                        RecordStatus = reader.GetString("RECORD_STATUS"),
                        Status = reader.GetString("TRANSACTION_STATUS"),
                        TransactionCode = reader.GetString("TRAN_CODE"),
                        LogObjectId = reader.GetInt64("TRANSACTION_OBJECT_ID"),
                        ActionType = reader.GetString("ACTION_TYPE"),
                        RequestType = reader.GetString("REQUEST_TYPE"),
                        UpdateDate = reader.GetDateTime("UPDATE_DATE"),
                        UpdatedUser = reader.GetString("UPDATE_USER"),
                        Duration = reader.GetInt32("DURATION")
                    };
                    SetDisplayProperties(log);
                    result.Add(log);
                }
            }

            return result;
        }

        protected override DataTable PrepareTable(List<ITransactionEntity> itemList)
        {
            throw new NotImplementedException();
        }

        void SetDisplayProperties(TransactionLog log)
        {
            //TransactionBase tb = TransactionManager.GetTransaction(log.TransactionCode);
            
            //string jsonString = JsonConvert.SerializeObject("");



        }


    }
}
