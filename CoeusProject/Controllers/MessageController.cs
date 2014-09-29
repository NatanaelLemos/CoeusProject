using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using CoeusProject.Facade;
using CoeusProject.Hubs;

namespace CoeusProject.Controllers
{
    public class MessageController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult SendMessage(Int32 idGroup, String txMessage)
        {
            try
            {
                Mensagem sentMessage = new Mensagem
                    {
                        IdUsuario = Convert.ToInt32(HttpContext.User.Identity.Name),
                        TxMensagem = txMessage,
                        IdGrupo = idGroup,
                        DtMensagem = DateTime.Now
                    };

                sentMessage.Encrypt(idGroup, _context);

                _context.Mensagens.Add(sentMessage);
                _context.SaveChanges();

                sentMessage.Grupo = _context.Grupos.Where(g => g.IdGrupo == sentMessage.IdGrupo).FirstOrDefault().Decrypt();
                new Chat().Send(sentMessage);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
        }

        public ActionResult GetGrupoMessages(Int32 idGrupo)
        {
            IQueryable<Mensagem> mensagens = null;

            if (idGrupo > 0)
            {
                mensagens = _context.Mensagens.Where(m => m.IdGrupo == idGrupo)
                                              .Include(m => m.Usuario)
                                              .OrderBy(m=>m.DtMensagem)
                                              .Skip(Math.Max(0, _context.Mensagens.Count() - 100))
                                              .Take(100);
            }

            if (mensagens == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json(mensagens.Decrypt().OrderBy(o=>o.DtMensagem).Select(m => new
            {
                IdMensagem = m.IdMensagem,
                TxMensagem = m.TxMensagem,
                NmPessoa = m.Usuario.NmPessoa,
                DtMensagem = m.DtMensagem.ToString("dd/MM/yyyy HH:mm")
            }), JsonRequestBehavior.AllowGet);
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