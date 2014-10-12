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

        private JsonResult GetAjaxVideos(DataSourceRequest request, Int32 idUsuario)
        {
            IQueryable<Video> videos = _context.Videos.Include(v => v.Objeto).Include(v => v.Objeto.Salt).Where(o => o.Objeto.IdUsuario == idUsuario);
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

        public ActionResult AjaxReadVideos(DataSourceRequest request, Int32 idUsuario)
        {
            return GetAjaxVideos(request, idUsuario);
        }

        [OutputCache(Duration=0, NoStore=true)]
        public ActionResult CreatePartial()
        {
            Video video = new Video() 
            { 
                Objeto = new Objeto(), 
                TxUrl = Sequence.GetSequence("video").ToString(), 
                TxUrlPoster = Sequence.GetSequence("foto").ToString()
            };
            
            String physicalPathVideo = Server.MapPath("~/User_Data/") + video.TxUrl;
            String physicalPathPoster = Server.MapPath("~/User_Data/") + video.TxUrlPoster;

            if (System.IO.File.Exists(physicalPathVideo + ".mp4"))
            {
                System.IO.File.Delete(physicalPathVideo + ".mp4");
            }
            else if (System.IO.File.Exists(physicalPathVideo + ".webm"))
            {
                System.IO.File.Delete(physicalPathVideo + ".webm");
            }
            else if (System.IO.File.Exists(physicalPathVideo + ".ogg"))
            {
                System.IO.File.Delete(physicalPathVideo + ".ogg");
            }

            if (System.IO.File.Exists(physicalPathPoster + ".png"))
            {
                System.IO.File.Delete(physicalPathPoster + ".png");
            }

            System.IO.File.Copy(Server.MapPath("~/Images/noPoster.png"), physicalPathPoster);
            return View("_VideoEditPartial", video);
        }

        [HttpPost]
        public ActionResult Create(String nmObjeto, String txDescricao, String txUrl, String txUrlPoster)
        {
            try
            {
                Video video = new Video()
                {
                    Objeto = new Objeto()
                    {
                        IdUsuario = AccountFacade.GetLoggedInUser().IdUsuario,
                        Salt = Salt.GetSalt(),
                        NmObjeto = nmObjeto,
                        TxDescricao = txDescricao
                    },
                    TxUrl = txUrl,
                    TxUrlPoster = (new FileController()).FormatPoster(txUrlPoster)
                };

                _context.Videos.Add(video.Encrypt(_context));
                _context.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult EditPartial(Int32 idVideo)
        {
            Video video =  _context.Videos.Where(v=>v.IdVideo == idVideo).Include(v=>v.Objeto).FirstOrDefault();
            if (video == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "Vídeo não encontrado");
            }

            video.Decrypt();
            return View("_VideoWatchPartial", video);
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

        public ActionResult Delete(DataSourceRequest request, Video video)
        {
            Video deletedVideo = _context.Videos.Where(v => v.IdVideo == video.IdVideo).FirstOrDefault();
            Objeto deletedObject = _context.Objetos.Where(o => o.IdObjeto == deletedVideo.IdObjeto).FirstOrDefault();

            deletedVideo.Decrypt();

            FileController.RemoveFile(deletedVideo.TxUrl);
            _context.Videos.Remove(deletedVideo);
            _context.Objetos.Remove(deletedObject);

            _context.SaveChanges();

            return GetAjaxVideos(request, AccountFacade.GetLoggedInUser().IdUsuario);
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
