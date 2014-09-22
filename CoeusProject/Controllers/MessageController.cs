using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CoeusProject.Controllers
{
    public class MessageController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult SendMessage(Int32 idGroup, String txMessage)
        {
            Mensagem sentMessage = new Mensagem 
                {
                    IdUsuario = Convert.ToInt32(HttpContext.User.Identity.Name),
                    TxMensagem = txMessage,
                    IdGrupo = 1
                };
            _context.Mensagens.Add(sentMessage);
            _context.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}