using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Messaging
{
    public class MessageJsonConverter : JsonConverter<RequestMessage>
    {



        public override RequestMessage ReadJson(JsonReader reader, Type objectType, RequestMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            RequestMessage request = new RequestMessage();

            JObject messageJo = JObject.Load(reader);
            if (!messageJo.ContainsKey("MessageHeader"))
                throw new Exception(string.Format("Can not convert request object. Message header is missing{0}{1}",Environment.NewLine, messageJo.ToString()));

            MessageHeader hdr = messageJo["MessageHeader"].ToObject<MessageHeader>();
            request.MessageHeader = hdr;

            if(messageJo.ContainsKey("MessageDataExtension"))
                request.MessageDataExtension = messageJo["MessageDataExtension"].ToObject<Dictionary<string, string>>();
            if (messageJo.ContainsKey("MessageTag"))
                request.MessageTag = messageJo["MessageTag"].ToObject<string>();

            if (messageJo.ContainsKey("TransactionEntityList"))
                request.TransactionEntityList = GetTransactionEntities(messageJo["TransactionEntityList"], hdr);

            return request;
        }

        List<ITransactionEntity> GetTransactionEntities(JToken entityTokens, MessageHeader header)
        {
            List<ITransactionEntity> result = new List<ITransactionEntity>();

            List<JToken> items = entityTokens.Children().ToList();


            string transactionCode = header.TransactionCode;
            TransactionContext context = TransactionManager.CreateContext(header.UserName);
            TransactionBase transaction = new TransactionManager(context).GetTransaction(transactionCode);

            foreach(JToken token in items)
            {
                transaction.Deserialize(token);
                result.Add(context.TransactionObject as ITransactionEntity);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, RequestMessage value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
