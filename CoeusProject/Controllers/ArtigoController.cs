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

        private JsonResult GetAjaxArtigos(DataSourceRequest request, Int32 idUsuario)
        {
            IQueryable<Artigo> artigos = _context.Artigos.Include(a => a.Objeto)
                                                        .Include(a => a.Objeto.Avaliacoes)
                                                        .Include(a => a.Objeto.Salt)
                                                        .Where(o => o.Objeto.IdUsuario == idUsuario);
            DataSourceResult result = artigos.Decrypt().Select(a => new Artigo
                                        {
                                            IdArtigo = a.IdArtigo,
                                            Objeto = new Objeto
                                            {
                                                IdObjeto = a.Objeto.IdObjeto,
                                                NmObjeto = a.Objeto.NmObjeto,
                                                QtAcessos = a.Objeto.QtAcessos,
                                                VlMediaAvaliacaoCalc = a.Objeto.VlMediaAvaliacao,
                                                TxDescricao = a.Objeto.TxDescricao
                                            }
                                        }).ToDataSourceResult(request);

            return Json(new DataSourceResult()
            {
                Data = result.Data,
                Total = result.Data.AsQueryable().Count()
            });
        }

        public ActionResult AjaxReadArtigos(DataSourceRequest request, Int32 idUsuario)
        {
            return GetAjaxArtigos(request, idUsuario);
        }

        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult CreatePartial()
        {
            Artigo artigo = new Artigo() { Objeto = new Objeto() { IdUsuario = AccountFacade.GetLoggedInUser().IdUsuario } };
            return View("_ArtigoEditPartial", artigo);
        }

        [HttpPost]
        public ActionResult Create(String nmObjeto, String txDescricao, String txArtigo, List<InteresseVM> tags)
        {
            try
            {
                if (tags == null || tags.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "É necessário selecionar ao menos 1 tema para o artigo");
                }

                Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

                List<Objeto> objetos = (new CoeusProjectContext()).Objetos.Where(o => o.IdUsuario == usuarioLogado.IdUsuario).Decrypt();

                if (objetos.Where(o => o.NmObjeto == nmObjeto).Count() > 0) return new HttpStatusCodeResult(HttpStatusCode.OK);

                Artigo artigo = new Artigo()
                {
                    Objeto = new Objeto()
                    {
                        IdUsuario = usuarioLogado.IdUsuario,
                        Salt = Salt.GetSalt(),
                        NmObjeto = nmObjeto,
                        TxDescricao = txDescricao
                    },
                    TxArtigo = txArtigo
                };

                artigo.Objeto.Temas = new List<Tema>();

                foreach (InteresseVM interesse in tags)
                {
                    artigo.Objeto.Temas.Add(_context.Temas.Where(t => t.NmTema == interesse.NmInteresse).FirstOrDefault());
                }

                _context.Artigos.Add(artigo.Encrypt(_context));

                Grupo artigoGrupo = new Grupo()
                {
                    IdObjeto = artigo.IdObjeto,
                    Salt = Salt.GetSalt(),
                    Usuarios = new List<Usuario> { _context.Usuarios.Where(u=>u.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault() },
                    NmGrupo = nmObjeto
                };

                _context.Grupos.Add(artigoGrupo.Encrypt(_context));
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
            Artigo artigo = _context.Artigos.Where(a => a.IdArtigo == idArtigo).FirstOrDefault();
            if (artigo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "Artigo não encontrado");
            }

            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            if (artigo.Objeto.IdUsuario != usuarioLogado.IdUsuario)
            {
                artigo.Objeto.QtAcessos++;
                _context.Entry(artigo).State = EntityState.Modified;
                _context.SaveChanges();

                Int32 noAvUsuario = 0;
                Avaliacao usuarioAvaliacao = artigo.Objeto.Avaliacoes.Where(a => a.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault();
                if (usuarioAvaliacao != null)
                {
                    noAvUsuario = usuarioAvaliacao.NoAvaliacao;
                }
                ViewBag.noAvUsuario = noAvUsuario;
            }

            artigo.Decrypt();
            return View("_ArtigoEditPartial", artigo);
        }

        [HttpPost]
        public ActionResult Edit(Int32 idArtigo, String nmObjeto, String txDescricao, String txArtigo, List<InteresseVM> tags)
        {
            try
            {
                if (tags == null || tags.Count == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "É necessário selecionar ao menos 1 tema para o artigo");
                }

                Artigo artigo = _context.Artigos.Where(a => a.IdArtigo == idArtigo).Include(a => a.Objeto).FirstOrDefault().Decrypt(_context);
                artigo.Objeto.NmObjeto = nmObjeto;
                artigo.Objeto.TxDescricao = txDescricao;

                artigo.TxArtigo = txArtigo;
                artigo.Encrypt(_context);

                artigo.Objeto.Temas = new List<Tema>();
                foreach (InteresseVM interesse in tags)
                {
                    artigo.Objeto.Temas.Add(_context.Temas.Where(t => t.NmTema == interesse.NmInteresse).FirstOrDefault());
                }


                _context.Entry(artigo).State = EntityState.Modified;
                _context.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
        }

        public ActionResult Delete(DataSourceRequest request, Artigo artigo)
        {
            Artigo artigoDel = _context.Artigos.Where(a => a.IdArtigo == artigo.IdArtigo).FirstOrDefault();
            if (artigoDel == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Artigo não encontrado");

            Objeto objeto = _context.Objetos.Include(o=>o.Temas).Where(o => o.IdObjeto == artigoDel.IdObjeto).FirstOrDefault();
            if (objeto.Temas != null)
            {
                objeto.Temas.Clear();
            }

            Grupo grupo = _context.Grupos.Include(g=>g.Usuarios).Where(g => g.IdObjeto != null && g.IdObjeto == objeto.IdObjeto).FirstOrDefault();
            if(grupo != null)
            {
                if (grupo.Usuarios != null)
                {
                    grupo.Usuarios.Clear();
                }
                _context.Grupos.Remove(grupo);
            }

            _context.Artigos.Remove(artigoDel);
            _context.Objetos.Remove(objeto);

            _context.SaveChanges();

            return GetAjaxArtigos(request, AccountFacade.GetLoggedInUser().IdUsuario);
        }

        public ActionResult Avaliacao(Int32 idArtigo, Int32 noAvaliacao)
        {
            Artigo artigo = _context.Artigos.Where(a => a.IdArtigo == idArtigo).Include(a => a.Objeto).FirstOrDefault();
            if (artigo == null) return new HttpStatusCodeResult(HttpStatusCode.OK);

            Objeto objeto = artigo.Objeto;

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
            ViewBag.TpObjeto = "Artigo";
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
