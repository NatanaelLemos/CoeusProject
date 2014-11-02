using CoeusProject.Facade;
using CoeusProject.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CoeusProject.Controllers
{
    public class GrupoController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();
        public ActionResult CreatePartial()
        {
            return View("_GrupoEditPartial", new Grupo());
        }

        public ActionResult EditPartial(Int32 IdGrupo)
        {
            Grupo grupo = _context.Grupos.Where(g=>g.IdGrupo == IdGrupo).FirstOrDefault();
            return View("_GrupoEditPartial", grupo);
        }

        public ActionResult GetGrupoUsuariosList(Int32 IdGrupo)
        {
            return Json(_context.Grupos.Where(g=>g.IdGrupo == IdGrupo).Include(g=>g.Usuarios)
                .FirstOrDefault().Usuarios.Decrypt().Select(u=>new
                {
                    IdUsuario = u.IdUsuario,
                    NmPessoa = u.NmPessoa,
                    NmFoto = u.NmFoto,
                    NmThumbFoto = u.NmThumbFoto
                }));
        }

        [HttpPost]
        public ActionResult Create(String nmGrupo, List<Usuario> usuarios)
        {
            try
            {
                if (String.IsNullOrEmpty(nmGrupo))
                {
                    throw new Exception("O nome do grupo é obrigatório");
                }

                if (usuarios == null || usuarios.Count() == 0)
                {
                    throw new Exception("É necessário selecionar algum usuário para o grupo");
                }

                Grupo grupo = new Grupo
                {
                    NmGrupo = nmGrupo,
                    Usuarios = new List<Usuario>(),
                    Salt = Salt.GetSalt()
                };

                foreach (Usuario usuario in usuarios)
                {
                    Usuario grupoUser = _context.Usuarios.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    if (grupoUser == null || grupo.Usuarios.Contains(grupoUser)) continue;

                    grupo.Usuarios.Add(grupoUser);
                }

                Usuario usuarioLogado = AccountFacade.GetLoggedInUser();
                if (grupo.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).Count() == 0)
                {
                    grupo.Usuarios.Add(_context.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault());
                }

                grupo.Encrypt();
                _context.Grupos.Add(grupo);
                _context.SaveChanges();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(Int32 idGrupo, String nmGrupo, List<Usuario> usuarios)
        {
            try
            {
                Grupo grupo = _context.Grupos.Where(g => g.IdGrupo == idGrupo).Include(g=>g.Usuarios).FirstOrDefault().Decrypt();
                if (grupo == null)
                {
                    throw new Exception("Grupo inexistente");
                }

                List<Usuario> usuariosGrupo = grupo.Usuarios.ToList();

                if (String.IsNullOrEmpty(nmGrupo))
                {
                    throw new Exception("Nome inválido para o grupo");
                }

                if (usuarios == null || usuarios.Count() == 0)
                {
                    throw new Exception("É necessário selecionar algum usuário para o grupo");
                }

                grupo.NmGrupo = nmGrupo;

                for (int i = usuariosGrupo.Count() - 1; i >= 0; i--)
                {
                    if (usuarios.Where(u => u.IdUsuario == usuariosGrupo[i].IdUsuario).Count() == 0)
                    {
                        grupo.Usuarios.Remove(usuariosGrupo[i]);
                    }
                }

                for (int i = usuarios.Count() - 1; i >= 0; i--)
                {
                    if (usuariosGrupo.Where(ug => ug.IdUsuario == usuarios[i].IdUsuario).Count() == 0)
                    {
                        Int32 idUsuarioAdded = usuarios[i].IdUsuario;
                        grupo.Usuarios.Add(_context.Usuarios.Where(u=>u.IdUsuario == idUsuarioAdded).FirstOrDefault());
                    }
                }

                Usuario usuarioLogado = AccountFacade.GetLoggedInUser();
                if (grupo.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).Count() == 0)
                {
                    grupo.Usuarios.Add(_context.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault());
                }

                grupo.Encrypt();
                _context.Entry(grupo).State = EntityState.Modified;
                _context.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, ErrorFacade.GetErrorMessage(ex));
            }
        }

        public ActionResult GetUserWhereNmPessoaStartsWith(String nmPessoa)
        {
            if (String.IsNullOrEmpty(nmPessoa))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            List<Usuario> usuarios = _context.Usuarios.ToList();
            List<Usuario> usuariosRet = new List<Usuario>();

            foreach (Usuario usuario in usuarios)
            {
                Usuario retUsuario = usuario.Decrypt();
                if (retUsuario.NmPessoa.ToUpper().StartsWith(nmPessoa.ToUpper()) ||
                    retUsuario.TxEmail.ToUpper().StartsWith(nmPessoa.ToUpper()))
                {
                    usuariosRet.Add(retUsuario);
                }
            }

            if (usuarios == null || usuarios.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json(usuariosRet.Select(u => new
            {
                IdUsuario = u.IdUsuario,
                NmPessoa = u.NmPessoa,
                TxEmail = u.TxEmail,
                NmFoto = u.NmFoto,
                NmThumbFoto = u.NmThumbFoto
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrupoWhereNmGrupoStartsWith(String nmGrupo)
        {
            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            List<Grupo> grupos = _context.Grupos.Where(g => g.Usuarios.Any(u => u.IdUsuario == usuarioLogado.IdUsuario) && g.IdObjeto == null)
                                       .Include(g => g.Usuarios).Decrypt();

            if (!String.IsNullOrEmpty(nmGrupo))
            {
                grupos = grupos.Where(g => g.NmGrupo.StartsWith(nmGrupo)).ToList();
            }

            if (grupos == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json(grupos.Select(g => new
            {
                IdGrupo = g.IdGrupo,
                NmGrupo = g.NmGrupo
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddObjectGroup(Int32 idObjeto)
        {
            Int32 idUsuarioLogado = AccountFacade.GetLoggedInUser().IdUsuario;
            List<Grupo> gruposFilter = _context.Grupos.Where(g => g.IdObjeto == null &&
                                        g.Usuarios.Any(u => u.IdUsuario == idUsuarioLogado)).OrderBy(g => g.NmGrupo).Decrypt();

            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            Grupo grupoObj = null;

            while (grupoObj == null)
            {
                grupoObj = _context.Grupos.Include(g => g.Usuarios).Include(g=>g.Salt)
                            .Where(g => g.IdObjeto != null && g.IdObjeto == idObjeto).FirstOrDefault();

                _context = new CoeusProjectContext();
            }

            gruposFilter.Add(grupoObj.Decrypt());

            return Json(gruposFilter.Select(s => new Grupo
            {
                IdGrupo = s.IdGrupo,
                NmGrupo = s.NmGrupo
            }));
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