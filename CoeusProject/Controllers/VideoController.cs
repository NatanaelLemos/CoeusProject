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
using CoeusProject.ViewModels;

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
            IQueryable<Video> videos = _context.Videos.Include(v => v.Objeto)
                                        .Include(v=>v.Objeto.Avaliacoes)
                                        .Include(v => v.Objeto.Salt)
                                        .Where(o => o.Objeto.IdUsuario == idUsuario);
            DataSourceResult result = videos.Decrypt().Select(v => new Video
            {
                IdVideo = v.IdVideo,
                Objeto = new Objeto
                {
                    IdObjeto = v.Objeto.IdObjeto,
                    NmObjeto = v.Objeto.NmObjeto,
                    TxDescricao = v.Objeto.TxDescricao,
                    VlMediaAvaliacaoCalc = v.Objeto.VlMediaAvaliacao,
                    QtAcessos = v.Objeto.QtAcessos
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

            if (System.IO.File.Exists(physicalPathPoster))
            {
                System.IO.File.Delete(physicalPathPoster);
            }

            System.IO.File.Copy(Server.MapPath("~/Images/noPoster.png"), physicalPathPoster);
            return View("_VideoEditPartial", video);
        }

        [HttpPost]
        public ActionResult Create(String nmObjeto, String txDescricao, String txUrl, String txUrlPoster, List<InteresseVM> tags)
        {
            try
            {
                if (tags == null || tags.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "É necessário selecionar ao menos 1 tema para o vídeo");
                }

                Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

                List<Objeto> objetos = (new CoeusProjectContext()).Objetos.Where(o => o.IdUsuario == usuarioLogado.IdUsuario).Decrypt();

                if (objetos.Where(o => o.NmObjeto == nmObjeto).Count() > 0) return new HttpStatusCodeResult(HttpStatusCode.OK);

                Video video = new Video()
                {
                    Objeto = new Objeto()
                    {
                        IdUsuario = usuarioLogado.IdUsuario,
                        Salt = Salt.GetSalt(),
                        NmObjeto = nmObjeto,
                        TxDescricao = txDescricao
                    },
                    TxUrl = txUrl,
                    TxUrlPoster = (new FileController()).FormatPoster(txUrlPoster)
                };

                video.Objeto.Temas = new List<Tema>();

                foreach (InteresseVM interesse in tags)
                {
                    video.Objeto.Temas.Add(_context.Temas.Where(t => t.NmTema == interesse.NmInteresse).FirstOrDefault());
                }

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

            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();
            if (video.Objeto.IdUsuario != usuarioLogado.IdUsuario)
            {
                video.Objeto.QtAcessos++;
                _context.Entry(video).State = EntityState.Modified;
                _context.SaveChanges();

                Int32 noAvUsuario = 0;
                Avaliacao usuarioAvaliacao = video.Objeto.Avaliacoes.Where(a => a.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault();
                if (usuarioAvaliacao != null)
                {
                    noAvUsuario = usuarioAvaliacao.NoAvaliacao;
                }
                ViewBag.noAvUsuario = noAvUsuario;
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
            Video videoDel = _context.Videos.Where(v => v.IdVideo == video.IdVideo).FirstOrDefault();
            if (videoDel == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Vídeo não encontrado");

            Objeto objeto = _context.Objetos.Include(o => o.Temas).Where(o => o.IdObjeto == videoDel.IdObjeto).FirstOrDefault();
            if (objeto.Temas != null)
            {
                objeto.Temas.Clear();
            }

            Grupo grupo = _context.Grupos.Include(g => g.Usuarios).Where(g => g.IdObjeto != null && g.IdObjeto == objeto.IdObjeto).FirstOrDefault();
            if (grupo != null)
            {
                if (grupo.Usuarios != null)
                {
                    grupo.Usuarios.Clear();
                }
                _context.Grupos.Remove(grupo);
            }

            _context.Videos.Remove(videoDel);
            _context.Objetos.Remove(objeto);

            _context.SaveChanges();

            return GetAjaxVideos(request, AccountFacade.GetLoggedInUser().IdUsuario);
        
        }

        public ActionResult Avaliacao(Int32 idVideo, Int32 noAvaliacao)
        {
            Video video = _context.Videos.Where(a => a.IdVideo == idVideo).Include(a => a.Objeto).FirstOrDefault();
            if (video == null) return new HttpStatusCodeResult(HttpStatusCode.OK);

            Objeto objeto = video.Objeto;

            if (objeto.Avaliacoes == null)
            {
                objeto.Avaliacoes = new List<Avaliacao>();
            }

            Usuario usuarioLogado = AccountFacade.GetLoggedInUser().Encrypt();
            Avaliacao avaliacao = objeto.Avaliacoes.Where(a => a.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault();

            if (avaliacao == null)
            {
                objeto.Avaliacoes.Add(new Avaliacao { IdUsuario = usuarioLogado.IdUsuario, NoAvaliacao = noAvaliacao });
            }
            else
            {
                avaliacao.NoAvaliacao = noAvaliacao;
                _context.Entry(avaliacao).State = EntityState.Modified;
            }

            _context.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult GetObjetoPopup()
        {
            ViewBag.TpObjeto = "Video";
            return View("~/Views/Shared/_ObjetoPopupPartial.cshtml");
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
