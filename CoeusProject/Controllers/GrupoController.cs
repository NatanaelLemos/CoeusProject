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

        [HttpPost]
        public ActionResult Create(String nmGrupo, List<Usuario> usuarios)
        {
            try
            {
                if (String.IsNullOrEmpty(nmGrupo))
                {
                    throw new Exception("O nome do grupo é obrigatório");
                }

                Grupo grupo = new Grupo
                {
                    NmGrupo = nmGrupo,
                    Usuarios = new List<Usuario>()
                };

                foreach (Usuario usuario in usuarios)
                {
                    Usuario grupoUser = _context.Usuarios.Where(u => u.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    if (grupoUser == null || grupo.Usuarios.Contains(grupoUser)) continue;

                    grupo.Usuarios.Add(grupoUser);
                }

                _context.Grupos.Add(grupo);
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
                NmFoto = u.NmFoto
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrupoWhereNmGrupoStartsWith(String nmGrupo)
        {
            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            IQueryable<Grupo> grupos = null;

            if (String.IsNullOrEmpty(nmGrupo))
            {
                grupos = _context.Grupos.Where(g => g.Usuarios.Any(u => u.IdUsuario == usuarioLogado.IdUsuario) && g.IdObjeto == null)
                                                            .Include(g => g.Usuarios);
            }
            else
            {
                grupos = _context.Grupos.Where(g => g.Usuarios.Any(u => u.IdUsuario == usuarioLogado.IdUsuario) && g.IdObjeto == null
                                                                         && g.NmGrupo.StartsWith(nmGrupo))
                                                            .Include(g => g.Usuarios);
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
    }
}