using Koenig.Maestro.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Koenig.Maestro.Web.Controllers
{
    public class MaestroListController : Controller
    {
        // GET: MaestroList
        public ActionResult Orders()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Customers(CustomerViewRequest request)
        {
            return View();
        }

        public ActionResult Customers()
        {
            return View();
        }


        public ActionResult Products()
        {
            return View();
        }

        public ActionResult Units()
        {
            return View();
        }

        public ActionResult UnitTypes()
        {
            return View();
        }

        public ActionResult Regions()
        {
            return View();
        }

        public ActionResult CustomerProductUnits()
        {
            return View();
        }

    }
}