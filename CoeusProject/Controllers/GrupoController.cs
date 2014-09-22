using CoeusProject.Facade;
using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Create(Grupo grupo)
        {
            return Json("");
        }

        public ActionResult GetUserWhereNmPessoaStartsWith(String nmPessoa)
        {
            if(String.IsNullOrEmpty(nmPessoa))
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
                return Json("",JsonRequestBehavior.AllowGet);
            }

            return Json(usuariosRet.Select(u => new 
            { 
                IdUsuario = u.IdUsuario,
                NmPessoa = u.NmPessoa,
                TxEmail = u.TxEmail,
                NmFoto = u.NmFoto
            }), JsonRequestBehavior.AllowGet);
        }
    }
}