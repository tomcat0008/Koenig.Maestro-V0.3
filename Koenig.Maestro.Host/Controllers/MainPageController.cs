using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koenig.Maestro.Host.Controllers
{
    public class MainPageController : BaseController
    {
        // GET: MainPage
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string requestMessage)
        {

            return ExecuteMessage(requestMessage);

        }

        [HttpPost]
        public JsonResult Import(string requestMessage)
        {
            return ExecuteMessage(requestMessage);
        }

        [HttpPost]
        public JsonResult GetOrderId(string requestMessage)
        {
            return ExecuteMessage(requestMessage);
        }

        public JsonResult GetItem(string requestMessage)
        {
            return ExecuteMessage(requestMessage);
        }

        public JsonResult UpdateItem(string requestMessage)
        {
            return ExecuteMessage(requestMessage);
        }

        public JsonResult CreateItem(string requestMessage)
        {
            return ExecuteMessage(requestMessage);
        }


        JsonResult ExecuteMessage(string requestMessage)
        {
            ResponseMessage result = MaestroReceiver.ProcessRequest(requestMessage, HttpContext.ApplicationInstance.Context.Request.UserHostName);

            JsonResult jr = Json(result, JsonRequestBehavior.AllowGet);
            return jr;
        }

    }
}