using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoeusProject.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace CoeusProject.Controllers
{
    public class ArtigoController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult GetMainArtigos(Int32 idUsuario)
        {
            return View("MainArtigos", idUsuario);
        }

        public ActionResult GetLeftDivArtigos(Int32 idUsuario)
        {
            Usuario usuarioDono = _context.Usuarios.Where(u => u.IdUsuario == idUsuario).FirstOrDefault().Decrypt();
            return View("_ArtigosLeftPartial", usuarioDono);
        }

        public ActionResult AjaxReadArtigos(DataSourceRequest request, Int32 idUsuario)
        {
            IQueryable<Artigo> artigos = _context.Artigos.Include(a=>a.Objeto).Where(o=>o.Objeto.IdUsuario == idUsuario);
            DataSourceResult result = artigos.ToDataSourceResult(request);

            return Json(new DataSourceResult() 
            { 
                Data = result.Data,
                Total = result.Data.AsQueryable().Count()
            });
        }

        public ActionResult CreatePartial()
        {
            Artigo artigo = new Artigo() { Objeto = new Objeto()};
            return View("_ArtigoEditPartial", artigo);
        }

        [HttpPost]
        public ActionResult Create(String txArtigo)
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
