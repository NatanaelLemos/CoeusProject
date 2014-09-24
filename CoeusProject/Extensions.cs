using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoeusProject
{
    public static class Extensions
    {
        public static List<Mensagem> Decrypt(this IQueryable<Mensagem> mensagens)
        {
            if (mensagens == null) return new List<Mensagem>();
            if (mensagens.Count() == 0) return mensagens.ToList();

            mensagens = mensagens.OrderBy(m => m.Usuario.NmPessoa);

            List<Mensagem> mensagensRet = mensagens.ToList();

            Usuario encUsuario = mensagensRet[0].Usuario;
            Usuario decUsuario = mensagensRet[0].Usuario.Decrypt();

            foreach (Mensagem mensagem in mensagensRet)
            {
                if (mensagem.Usuario.NmPessoa != encUsuario.NmPessoa)
                {
                    encUsuario = mensagem.Usuario;
                    decUsuario = mensagem.Usuario.Decrypt();
                }

                mensagem.Decrypt(decUsuario);
            }
            return mensagensRet;
        }
    }
}