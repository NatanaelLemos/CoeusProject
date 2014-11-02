using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using CoeusProject.Facade;

namespace CoeusProject.Controllers
{
    public class SeguidorController : Controller
    {
        CoeusProjectContext _context = new CoeusProjectContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Follow(Int32 idObjeto)
        {
            Objeto objeto = _context.Objetos.Where(o => o.IdObjeto == idObjeto).FirstOrDefault();
            if (objeto == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Objeto não encontrado");

            Usuario usuario = _context.Usuarios.Include(u=>u.Seguidores).Where(u => u.IdUsuario == objeto.IdUsuario).FirstOrDefault();
            if (usuario == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Usuário não encontrado");

            if (usuario.Seguidores == null) usuario.Seguidores = new List<Usuario>();

            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            if (usuario.Seguidores.Where(s => s.IdUsuario == usuarioLogado.IdUsuario).Count() > 0) 
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Você já segue essa pessoa");

            usuario.Seguidores.Add(_context.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault());

            _context.Entry(usuario).State = EntityState.Modified;
            _context.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult GetFollowersIndex(Int32 idUsuario)
        {
            return View("_SeguidorPartial", GetFollowers(idUsuario));
        }

        public ActionResult GetFollowingIndex(Int32 idUsuario)
        {
            return View("_SeguidorPartial", GetFollowing(idUsuario));
        }

        private List<Usuario> GetFollowers(Int32 idUsuario)
        {
            IQueryable<Usuario> usuarios = _context.Usuarios.Where(u => u.IdUsuario == idUsuario)
                                            .Include(u=>u.Salt)
                                            .Include(u => u.Seguidores).SelectMany(u => u.Seguidores);

            List<Usuario> usuariosList = usuarios.ToList();

            return usuariosList.Select(u => new Usuario 
            {
                IdUsuario = u.IdUsuario,
                NmPessoa = u.NmPessoa,
                SnPessoa = u.SnPessoa,
                NmFoto = u.NmThumbFoto,
                TxEmail = u.TxEmail,
                IdSalt = u.IdSalt
            }).Decrypt(_context);
        }

        private List<Usuario> GetFollowing(Int32 idUsuario)
        {
            IQueryable<Usuario> usuarios = _context.Usuarios.Include(u => u.Salt).Where(u => u.Seguidores.Any(s => s.IdUsuario == idUsuario));

            List<Usuario> usuariosList = usuarios.ToList();

            return usuariosList.Select(u => new Usuario
            {
                IdUsuario = u.IdUsuario,
                NmPessoa = u.NmPessoa,
                SnPessoa = u.SnPessoa,
                NmFoto = u.NmThumbFoto,
                TxEmail = u.TxEmail,
                IdSalt = u.IdSalt
            }).Decrypt(_context);
        }
    }
}