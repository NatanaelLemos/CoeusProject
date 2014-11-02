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
using CoeusProject.ViewModels;

namespace CoeusProject.Controllers
{
    public class AccountController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult Login()
        {
            (new FileController()).CleanImages(_context);
            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();
            if (usuarioLogado != null)
            {
                //return View("Home/Index");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario usuario, Boolean flContinuarConectado)
        {
            if (ModelState.IsValid)
            {
                List<Usuario> usuarios = _context.Usuarios.Include(u => u.Salt).Decrypt();

                Usuario user = usuarios.Where(u => u.TxEmail == usuario.TxEmail).FirstOrDefault();

                if (user == null || user.PwUsuario != SecurityFacade.Encrypt(usuario.PwUsuario, user.Salt.BtSalt))
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

        public ActionResult Teste(Int32 IdTeste)
        {
            return View();
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

            String physicalPath = Server.MapPath("~/User_Data/") + usuario.NmFoto + ".png";

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            System.IO.File.Copy(Server.MapPath("~/Images/userNoPhoto.png"), physicalPath);

            return View(usuario);
        }

        public ActionResult RegisterPartial()
        {
            Usuario usuario = AccountFacade.GetLoggedInUser();

            if (usuario != null)
            {
                return Content("&nbsp;");
            }

            usuario = new Usuario();
            usuario.NmFoto = Sequence.GetSequence("foto").ToString();

            String physicalPath = Server.MapPath("~/User_Data/") + usuario.NmFoto + ".png";

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            System.IO.File.Copy(Server.MapPath("~/Images/userNoPhoto.png"), physicalPath);
            return View("~/Views/Account/_RegisterPartial.cshtml", usuario);
        }

        [HttpPost]
        public ActionResult Register(Usuario usuario, List<InteresseVM> interesses)
        {
            if ((new CoeusProjectContext()).Usuarios.Decrypt().Where(u => u.TxEmail == usuario.TxEmail).Count() > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "O e-mail informado já pertence à outro usuário");
            }

            if (interesses == null || interesses.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "É necessário selecionar ao menos um interesse");
            }

            usuario.Temas = new List<Tema>();
            foreach (InteresseVM interesse in interesses)
            {
                usuario.Temas.Add(_context.Temas.Where(t => t.NmTema == interesse.NmInteresse).FirstOrDefault());
            }

            usuario.NmFoto = (new FileController()).FormatImage(usuario.NmFoto);
            usuario.Salt = Salt.GetSalt(0, _context);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Usuarios.Add(usuario.Encrypt(_context));
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

        public ActionResult EditPartial()
        {
            Usuario usuario = AccountFacade.GetLoggedInUser();
            usuario.PwUsuario = String.Empty;
            return View("_EditPartial", usuario);
        }

        [HttpPost]
        public ActionResult Edit(Usuario usuario, List<InteresseVM> interesses)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuario usuarioEdit = _context.Usuarios.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault().Decrypt();
                    usuarioEdit.TxEmail = usuario.TxEmail;
                    usuarioEdit.NmPessoa = usuario.NmPessoa;
                    usuarioEdit.SnPessoa = usuario.SnPessoa;
                    usuarioEdit.PwUsuario = usuario.PwUsuario;

                    if (interesses != null && interesses.Count() > 0)
                    {
                        usuarioEdit.Temas = new List<Tema>();
                        foreach (InteresseVM interesse in interesses)
                        {
                            usuarioEdit.Temas.Add(_context.Temas.Where(t => t.NmTema == interesse.NmInteresse).FirstOrDefault());
                        }
                    }

                    _context.Entry(usuarioEdit.Encrypt()).State = EntityState.Modified;
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

        public ActionResult GetUsuarioByObjeto(Int32 idObjeto)
        {
            Objeto objeto = _context.Objetos.Where(o => o.IdObjeto == idObjeto)
                .Include(o => o.Usuario).FirstOrDefault();

            if(objeto == null || objeto.Usuario == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            objeto.Decrypt().Usuario.Decrypt();

            return Json(new 
            { 
                IdUsuario = objeto.Usuario.IdUsuario,
                NmPessoa = objeto.Usuario.NmPessoa,
                NmFoto = objeto.Usuario.NmFoto,
                NmThumbFoto = objeto.Usuario.NmThumbFoto
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsuarioRanking(Int32 idUsuario)
        {
            Usuario usuario = _context.Usuarios.Include(u=>u.Objetos).Where(u => u.IdUsuario == idUsuario).FirstOrDefault();
            if (usuario == null) return Json("0", JsonRequestBehavior.AllowGet);

            return Json(usuario.Rank(_context).ToString(), JsonRequestBehavior.AllowGet);
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