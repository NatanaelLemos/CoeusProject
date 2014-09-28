using CoeusProject.Facade;
using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoeusProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeftContent()
        {
            return View("_LeftContentPartial");
        }

        public ActionResult FeedContent()
        {
            return View("_ContentPartial");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
        //        _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}