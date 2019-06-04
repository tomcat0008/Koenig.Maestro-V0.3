using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity.Enums
{
    public struct MessageDataExtensionKeys
    {
        public static readonly string ID = "ID";
        public static readonly string CLEAN_ORDER_ITEMS = "CLEANORDERITEMS";
        public static readonly string REQUEST_TYPE = "REQUEST_TYPE";
        public static readonly string PERIOD = "PERIOD";
        public static readonly string BEGIN_DATE = "BEGIN_DATE";
        public static readonly string END_DATE = "END_DATE";
        public static readonly string CUSTOMER_ID = "CUSTOMER_ID";
        public static readonly string STATUS = "STATUS";
        public static readonly string TRANSACTION_CODE = "TRANSACTION_CODE";
        public static readonly string IMPORT_TYPE = "IMPORT_TYPE";
        public static readonly string EXPORT_TYPE = "EXPORT_TYPE";
        public static readonly string SEND_TO_QB = "SEND_TO_QB";
        public static readonly string CREATE_INVOICE = "CREATE_INVOICE";
        public static readonly string BATCH_ID = "BATCH_ID";
        public static readonly string NOT_INTEGRATED = "NOT_INTEGRATED";
        public static readonly string SAVE_FILE = "SAVE_FILE";
        public static readonly string REPORT_CODE = "REPORT_CODE";
        public static readonly string LIST_CODE = "LIST_CODE";
        public static readonly string INVOICE_GROUP = "INVOICE_GROUP";
    }

    public struct MessageDataExtensionValues
    {
        public static readonly string LIST_CODE_MERGE_INVOICE = "MERGE_INVOICE";
    }
}
