﻿using CoeusProject.Facade;
using CoeusProject.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}