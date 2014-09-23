using CoeusProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoeusProject
{
    public static class Extensions
    {
        public static List<Mensagem> Decrypt(this List<Mensagem> mensagens)
        {
            if (mensagens == null) return new List<Mensagem>();
            if (mensagens.Count() == 0) return mensagens;

            Usuario encUsuario = mensagens[0].Usuario;
            Usuario decUsuario = mensagens[0].Usuario.Decrypt();

            foreach (Mensagem mensagem in mensagens)
            {
                if (mensagem.Usuario.NmPessoa == encUsuario.NmPessoa)
                {
                    mensagem.Decrypt(decUsuario);
                }
                else
                {
                    encUsuario = mensagem.Usuario;
                    decUsuario = mensagem.Usuario.Decrypt();
                    mensagem.Decrypt(decUsuario);
                }
            }
            return mensagens;
        }
    }
}