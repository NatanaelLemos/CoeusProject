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

        //[OutputCache(Duration=0)]
        public ActionResult GetInteressePartial()
        {
            return View("_InteressePartial", DateTime.Now.Ticks);
        }

        public ActionResult GetInteresseItem(int index)
        {
            if (index == -1)
            {
                long longIndex = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;

                if (longIndex > int.MaxValue -1)
                {
                    longIndex -= int.MaxValue;
                }
                index = Convert.ToInt32(longIndex);
            }
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
            if (!String.IsNullOrEmpty(NmInteresse) && _context.Temas.Where(t => t.NmTema == NmInteresse).Count() == 0)
            {
                _context.Temas.Add(new Tema { NmTema = NmInteresse});
                _context.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}