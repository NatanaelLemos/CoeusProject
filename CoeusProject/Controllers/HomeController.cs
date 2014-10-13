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

        private IEnumerable<ObjetoVM> GetObjetos()
        {
            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();
            List<int> idUsuarioTemas = usuarioLogado.Temas.Select(t => t.IdTema).ToList();
            IQueryable<Objeto> objetos = _context.Objetos
                                    .Where(o => o.Temas.Any(t => idUsuarioTemas.Contains(t.IdTema)) ||
                                                o.IdUsuario == usuarioLogado.IdUsuario);

            return objetos.Decrypt().Select(o => new ObjetoVM(o));
        }

        public ActionResult GetAllContent()
        {
            return View("_AllContent", GetObjetos());
        }

        public ActionResult GetArtigosContent()
        {
            List<ObjetoVM> objetos = GetObjetos().ToList();

            for (Int32 i = objetos.Count() - 1; i >= 0; i--)
            {
                Int32 idObjeto = objetos[i].IdObjeto;
                if (_context.Artigos.Where(a => a.IdObjeto == idObjeto).Count() == 0)
                {
                    objetos.RemoveAt(i);
                }
            }
            
            return View("_AllContent", objetos);
        }

        public ActionResult GetVideosContent()
        {
            List<ObjetoVM> objetos = GetObjetos().ToList();

            for (Int32 i = objetos.Count() - 1; i >= 0; i--)
            {
                Int32 idObjeto = objetos[i].IdObjeto;
                if (_context.Videos.Where(v => v.IdObjeto == idObjeto).Count() == 0)
                {
                    objetos.RemoveAt(i);
                }
            }

            return View("_AllContent", objetos);
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