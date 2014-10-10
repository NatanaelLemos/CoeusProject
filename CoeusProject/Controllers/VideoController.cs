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
    public class VideoController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult GetMainVideos(Int32 idUsuario)
        {
            return View("MainVideos", idUsuario);
        }

        public ActionResult GetLeftDivVideos(Int32 idUsuario)
        {
            Usuario usuarioDono = _context.Usuarios.Where(u => u.IdUsuario == idUsuario).FirstOrDefault().Decrypt();
            return View("_VideosLeftPartial", usuarioDono);
        }

        public ActionResult AjaxReadVideos(DataSourceRequest request, Int32 idUsuario)
        {
            IQueryable<Video> videos = _context.Videos.Include(v=>v.Objeto).Include(v=>v.Objeto.Salt).Where(o=>o.Objeto.IdUsuario == idUsuario);
            DataSourceResult result = videos.Decrypt().Select(v => new Video
                                        {
                                            IdVideo = v.IdVideo,
                                            Objeto = new Objeto 
                                            {
                                                IdObjeto = v.Objeto.IdObjeto,
                                                NmObjeto = v.Objeto.NmObjeto,
                                                TxDescricao = v.Objeto.TxDescricao
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
            Video video = new Video() { Objeto = new Objeto(), TxUrl = Sequence.GetSequence("video").ToString()};
            String physicalPath = Server.MapPath("~/User_Data/") + video.TxUrl;

            if (System.IO.File.Exists(physicalPath + ".mp4"))
            {
                System.IO.File.Delete(physicalPath + ".mp4");
            }
            else if (System.IO.File.Exists(physicalPath + ".webm"))
            {
                System.IO.File.Delete(physicalPath + ".webm");
            }
            else if (System.IO.File.Exists(physicalPath + ".ogg"))
            {
                System.IO.File.Delete(physicalPath + ".ogg");
            }

            return View("_VideoEditPartial", video);
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
