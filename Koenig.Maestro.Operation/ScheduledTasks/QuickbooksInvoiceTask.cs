using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.QuickBooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.ScheduledTasks
{
    public class QuickbooksInvoiceTask
    {
        TransactionContext context;

        public QuickbooksInvoiceTask(TransactionContext context)
        {
            this.context = context;
        }

        

        public void Execute()
        {
            /*DateTime lastExecution = (DateTime)context.Bag["LAST_EXECUTION"];
            double intervall = (double)context.Bag["INTERVALL"];*/

            QuickBooksInvoiceAgent agent = new QuickBooksInvoiceAgent(context);
            List<ITransactionEntity> invoices = agent.Import();


        }


    }
}
