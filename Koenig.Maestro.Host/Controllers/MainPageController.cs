using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koenig.Maestro.Host.Controllers
{
    public class MainPageController : BaseController
    {
        TransactionProgressEventArgs e = new TransactionProgressEventArgs(0,0,"") ;
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

        public JsonResult DeleteItem(string requestMessage)
        {
            return ExecuteMessage(requestMessage);
        }

        
        private void Receiver_TransactionProgress(object sender, TransactionProgressEventArgs e)
        {
            this.e = e;
            this.Message();
        }

        public JsonResult CreateInvoice(string requestMessage)
        {
            MaestroReceiver receiver = new MaestroReceiver();
            receiver.TransactionProgress += Receiver_TransactionProgress;

            ResponseMessage result = receiver.ProcessRequest(requestMessage, HttpContext.ApplicationInstance.Context.Request.UserHostName);


            receiver.TransactionProgress -= Receiver_TransactionProgress;

            JsonResult jr = Json(result, JsonRequestBehavior.AllowGet);
            return jr;
        }

        JsonResult ExecuteMessage(string requestMessage)
        {
            MaestroReceiver receiver = new MaestroReceiver();
            ResponseMessage result = receiver.ProcessRequest(requestMessage, HttpContext.ApplicationInstance.Context.Request.UserHostName);

            JsonResult jr = Json(result, JsonRequestBehavior.AllowGet);
            return jr;
        }

        



        public ActionResult Message()
        {
            return Content("aaaaaaaa", "text/event-stream");
        }

    }
}