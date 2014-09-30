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
using CoeusProject.Facade;

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
            IQueryable<Artigo> artigos = _context.Artigos.Include(a=>a.Objeto).Include(a=>a.Objeto.Salt).Where(o=>o.Objeto.IdUsuario == idUsuario);
            DataSourceResult result = artigos.Decrypt().Select(a => new Artigo
                                        {
                                            IdArtigo = a.IdArtigo,
                                            Objeto = new Objeto 
                                            {
                                                IdObjeto = a.Objeto.IdObjeto,
                                                NmObjeto = a.Objeto.NmObjeto,
                                                TxDescricao = a.Objeto.TxDescricao
                                            }
                                        }).ToDataSourceResult(request);

            return Json(new DataSourceResult() 
            { 
                Data = result.Data,
                Total = result.Data.AsQueryable().Count()
            });
        }

        [OutputCache(Duration=0, NoStore=true)]
        public ActionResult CreatePartial()
        {
            Artigo artigo = new Artigo() { Objeto = new Objeto()};
            return View("_ArtigoEditPartial", artigo);
        }

        [HttpPost]
        public ActionResult Create(String nmObjeto, String txDescricao, String txArtigo)
        {
            try
            {
                Artigo artigo = new Artigo()
                {
                    Objeto = new Objeto()
                    {
                        IdUsuario = AccountFacade.GetLoggedInUser().IdUsuario,
                        Salt = Salt.GetSalt(),
                        NmObjeto = nmObjeto,
                        TxDescricao = txDescricao
                    },
                    TxArtigo = txArtigo
                };

                _context.Artigos.Add(artigo.Encrypt(_context));
                _context.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult EditPartial(Int32 idArtigo)
        {
            Artigo artigo = _context.Artigos.Where(a=>a.IdArtigo == idArtigo).FirstOrDefault();
            if (artigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "Artigo não encontrado");
            }

            artigo.Decrypt();
            return View("_ArtigoEditPartial", artigo);
        }

        [HttpPost]
        public ActionResult Edit(Int32 idArtigo, String nmObjeto, String txDescricao, String txArtigo)
        {
            try
            {
                Artigo artigo = _context.Artigos.Where(a => a.IdArtigo == idArtigo).Include(a=>a.Objeto).FirstOrDefault().Decrypt(_context);
                artigo.Objeto.NmObjeto = nmObjeto;
                artigo.Objeto.TxDescricao = txDescricao;
                
                artigo.TxArtigo = txArtigo;
                artigo.Encrypt(_context);

                _context.Entry(artigo).State = EntityState.Modified;
                _context.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
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
