using CoeusProject.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using CoeusProject.Facade;
using System.Data.Entity;
using System.Web;
using System.Collections.Generic;

namespace CoeusProject.Controllers
{
    public class AccountController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario usuario, Boolean flContinuarConectado)
        {
            if (ModelState.IsValid)
            {
                usuario.Encrypt();
                Usuario user = _context.Usuarios.Where(u => u.TxEmail == usuario.TxEmail && u.PwUsuario == usuario.PwUsuario).FirstOrDefault();

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "E-mail ou senha inválidos");
                }

                AccountFacade.Login(user, flContinuarConectado);
                return Content(Url.Action("Index", "Home"));
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ModelState));
        }

        [HttpPost]
        public ActionResult Logoff()
        {
            AccountFacade.Logoff();
            return Content(Url.Action("Index", "Home"));
        }

        public ActionResult Register()
        {
            Usuario usuario = AccountFacade.GetLoggedInUser();

            if (usuario != null)
            {
                return View("Edit");
            }

            usuario = new Usuario();
            usuario.NmFoto = Sequence.GetSequence("foto").ToString();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Register(Usuario usuario)
        {
            usuario.NmFoto = (new FileController()).FormatImage(usuario.NmFoto);
            Usuario encUsuario = usuario.Encrypt();
            if (_context.Usuarios.Where(u => u.TxEmail == encUsuario.TxEmail).FirstOrDefault() != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "O email informado já pertence à outro usuário");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Usuarios.Add(usuario);
                    _context.SaveChanges();

                    AccountFacade.Login(usuario, false);
                    return Content(Url.Action("Index", "Home"));
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ModelState));
        }

        public ActionResult Edit()
        {
            return View();
        }

        //[OutputCache(Duration=0, NoStore=true)]
        public ActionResult EditPartial()
        {
            Usuario usuario = AccountFacade.GetLoggedInUser();
            usuario.PwUsuario = String.Empty;
            return View("_EditPartial", usuario);
        }

        [HttpPost]
        public ActionResult Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(usuario.Encrypt()).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Content(Url.Action("Index", "Home"));
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ModelState));
        }
    }
}