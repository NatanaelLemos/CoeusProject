using CoeusProject.Models;
using CoeusProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CoeusProject.Controllers
{
    public class InteresseController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();
        public ActionResult GetInteressePartial()
        {
            return View("_InteressePartial");
        }

        public ActionResult GetInteresseItem(int index)
        {
            return View("_InteresseItemPartial", ++index);
        }

        public ActionResult GetInteresses(String text)
        {
            IQueryable<Tema> temas = _context.Temas.Where(t => t.NmTema.StartsWith(text));

            return Json(temas.Select(t => new 
            { 
                IdTema = t.IdTema,
                NmTema = t.NmTema
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInteresse(String NmInteresse)
        {
            if (_context.Temas.Where(t => t.NmTema == NmInteresse).Count() == 0)
            {
                _context.Temas.Add(new Tema { NmTema = NmInteresse});
                _context.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}