using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koenig.Maestro.Web.Controllers
{
    public class MaestroController : Controller
    {
        // GET: Maestro
        public ActionResult Index()
        {
            System.Diagnostics.Debugger.Break();
            return View();
        }
    }
}