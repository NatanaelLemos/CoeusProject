using CoeusProject.Facade;
using CoeusProject.Models;
using CoeusProject.ViewModels;
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
        CoeusProjectContext _context = new CoeusProjectContext();

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

        public ActionResult GetAllContent()
        {
            return View("_AllContent", _context.Objetos.Decrypt().Select(o=>new ObjetoVM(o)));
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