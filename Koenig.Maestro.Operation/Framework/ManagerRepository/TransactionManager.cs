using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Messaging;
using Koenig.Maestro.Operation.TransactionRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class TransactionManager : ManagerBase
    {
        public TransactionManager() : base(null) { }

        public TransactionManager(TransactionContext context) : base(context) { }

        static Dictionary<Guid, TransactionContext> contextCache = new Dictionary<Guid, TransactionContext>();

        public TransactionBase GetTransaction(string transactionCode)
        {
            TransactionDefinition td = TransactionCache.Instance.Get(transactionCode);

            Type type = Type.GetType(td.CodeBase);
            if (type == null)
                throw new Exception(string.Format("Codebase `{0}` of transaction `{1}` could not be found", td.CodeBase, transactionCode));
            TransactionBase result = (TransactionBase)Activator.CreateInstance(type, new object[] { context });
            
            return result;
        }
        public static TransactionContext CreateContext(string userName)
        {
            TransactionContext context = new TransactionContext()
            {
                UserName = userName,
                Bag = new Dictionary<string, object>()
                
            };
            return context;
        }

        public static TransactionContext CreateContext(string userName, RequestMessage message)
        {
            Guid ticket = Guid.NewGuid();
            TransactionContext context = new TransactionContext()
            {
                Database = new Data.Database(),
                TransactionId = ticket,
                UserName = userName,
                RequestMessage = message,
                Warnings = new List<string>(),
                Bag = new Dictionary<string, object>()
            };
            contextCache.Add(ticket, context);

            return context;
        }

        public static TransactionContext GetContext(Guid guid)
        {
            TransactionContext result = null;
            if (contextCache.ContainsKey(guid))
                result = contextCache[guid];
            return result;
        }

        public static void DisposeTransaction(TransactionBase tb)
        {
            try
            {
                Guid ticket = tb.Context.TransactionId;
                TransactionContext result = contextCache[ticket];
                result.Database.Dispose();
                contextCache.Remove(ticket);
                tb.Dispose();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Exception while transaction dispose");
            }
        }
    }
}
