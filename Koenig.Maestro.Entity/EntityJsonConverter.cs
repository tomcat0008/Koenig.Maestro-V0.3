using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    public class EntityJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType is ITransactionEntity;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            writer.WriteStartObject();

            List<PropertyInfo> piList = value.GetType().GetProperties().Where(p => p.GetCustomAttributes<JsonConversionTarget>().Any()).ToList();
            piList.ForEach(delegate (PropertyInfo pi)
            {
                if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(String) || pi.PropertyType == typeof(Decimal) )
                {
                    writer.WritePropertyName(pi.Name);
                    serializer.Serialize(writer, pi.GetValue(value));
                }
                else if(pi.PropertyType.IsArray)
                {

                }
                else if(pi.PropertyType is ITransactionEntity)
                {

                }
            });
            
            writer.WriteEndObject();


        }

    }
}
