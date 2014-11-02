using CoeusProject.Facade;
using CoeusProject.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CoeusProject.Hubs
{
    [HubName("chat")]
    public class Chat : Hub
    {
        public void Send(Mensagem mensagem)
        {
            Mensagem decMensagem = mensagem.Decrypt();

            if (decMensagem.Usuario == null)
            {
                decMensagem.Usuario = new CoeusProjectContext().Usuarios.Where(u => u.IdUsuario == decMensagem.IdUsuario).FirstOrDefault().Decrypt();
            }

            JsonResult result = new JsonResult();
            result.Data = new
            {
                IdMensagem = decMensagem.IdMensagem,
                TxMensagem = decMensagem.TxMensagem,
                DtMensagem = decMensagem.DtMensagem.ToString("dd/MM/yyyy HH:mm"),
                IdUsuario = decMensagem.IdUsuario,
                NmPessoa = decMensagem.Usuario.NmPessoa,
                IdGrupo = mensagem.IdGrupo.ToString(),
                NmGrupo = mensagem.Grupo == null ? "" : mensagem.Grupo.NmGrupo
            };

            var context = GlobalHost.ConnectionManager.GetHubContext<Chat>();
            context.Clients.Group(mensagem.IdGrupo.ToString()).notify(result);
        }

        public void Join()
        {
            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            foreach (Grupo grupo in usuarioLogado.Grupos)
            {
                Groups.Add(Context.ConnectionId, grupo.IdGrupo.ToString());
            }
        }

        public void JoinObjectChat(Int32 idObject = 0)
        {
            Boolean changed = false;
            CoeusProjectContext context = new CoeusProjectContext();
            Usuario usuarioLogado = AccountFacade.GetLoggedInUser();

            IQueryable<Grupo> userObjectGroup = context.Grupos.Where(g => g.IdObjeto != null && g.Usuarios.Any(u => u.IdUsuario == usuarioLogado.IdUsuario));

            List<Grupo> gruposList = userObjectGroup.ToList();

            foreach (Grupo grupo in userObjectGroup)
            {
                Groups.Remove(Context.ConnectionId, grupo.IdGrupo.ToString());
            }

            Objeto objeto = context.Objetos.Where(o => o.IdObjeto == idObject).Include(o => o.Salt).FirstOrDefault().Decrypt();
            Grupo grupoObj = context.Grupos.Include(g => g.Usuarios).Where(g => g.IdObjeto != null && g.IdObjeto == idObject).FirstOrDefault();

            if (grupoObj == null)
            {
                changed = true;

                grupoObj = new Grupo()
                {
                    IdObjeto = idObject,
                    Salt = Salt.GetSalt(),
                    NmGrupo = objeto.NmObjeto
                };

                context.Grupos.Add(grupoObj.Encrypt());
            }

            if (grupoObj.Usuarios == null) grupoObj.Usuarios = new List<Usuario>();

            if (grupoObj.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).Count() == 0)
            {
                changed = true;
                grupoObj.Usuarios.Add(context.Usuarios.Where(u => u.IdUsuario == usuarioLogado.IdUsuario).FirstOrDefault());
            }

            if (changed)
            {
                objeto.Encrypt();
                context.Entry(objeto).State = EntityState.Modified;

                context.SaveChanges();
            }

            Groups.Add(Context.ConnectionId, grupoObj.IdGrupo.ToString());
        }
    }
}