using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Koenig.Maestro.Operation.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Koenig.Maestro.Operation.Framework
{
    public class MaestroReceiver
    {

        public event TransactionProgressEventHandler TransactionProgress;

        public ResponseMessage ProcessRequest(string message, string hostName)
        {
            RequestMessage request = JsonConvert.DeserializeObject<RequestMessage>(message, new MessageJsonConverter());
            request.MessageHeader.HostName = string.IsNullOrEmpty( hostName) ? string.Empty : hostName;
            MessageBroker broker = new MessageBroker();
            broker.TransactionProgress += Broker_TransactionProgress;
            ResponseMessage response = broker.Execute(request);
            return response;
        }

        public void OnTransactionProgress(TransactionProgressEventArgs e)
        {
            if (TransactionProgress != null)
                this.TransactionProgress(this, e);
        }


        private void Broker_TransactionProgress(object sender, TransactionProgressEventArgs e)
        {
            OnTransactionProgress(e);
            Console.WriteLine(e.Message);
        }
    }
}
