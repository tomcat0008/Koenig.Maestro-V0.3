using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koenig.Maestro.Host.Controllers
{
    public class MaestroJsonResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            
            Converters = new List<JsonConverter> {  new EntityJsonConverter() }

        };
        
        public override void ExecuteResult(ControllerContext context)
        {
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("GET request not allowed");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";


            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data == null)
            {
                return;
            }
            response.Write(JsonConvert.SerializeObject(this.Data, Settings));


        }
    }
}