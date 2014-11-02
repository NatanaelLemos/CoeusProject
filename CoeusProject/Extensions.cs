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
        public static List<Mensagem> Decrypt(this IQueryable<Mensagem> mensagens, CoeusProjectContext Context = null)
        {
            if (mensagens == null) return new List<Mensagem>();
            if (mensagens.Count() == 0) return mensagens.ToList();

            mensagens = mensagens.OrderBy(m => m.Usuario.NmPessoa);

            List<Mensagem> mensagensRet = mensagens.ToList();

            Usuario encUsuario = mensagensRet[0].Usuario;
            Usuario decUsuario = mensagensRet[0].Usuario.Decrypt();

            if (Context == null)
            {
                Context = new CoeusProjectContext();
            }

            foreach (Mensagem mensagem in mensagensRet)
            {
                if (mensagem.Usuario.NmPessoa != encUsuario.NmPessoa)
                {
                    encUsuario = mensagem.Usuario;
                    decUsuario = mensagem.Usuario.Decrypt();
                }

                mensagem.Decrypt(0, decUsuario, Context);
            }
            return mensagensRet;
        }

        public static List<Usuario> Decrypt(this IQueryable<Usuario> usuarios, CoeusProjectContext context = null)
        {
            if (context == null) context = new CoeusProjectContext();

            List<Usuario> usuariosRet = new List<Usuario>();
            foreach (Usuario usuario in usuarios)
            {
                usuariosRet.Add(usuario.Decrypt(context));
            }
            return usuariosRet;
        }

        public static List<Usuario> Decrypt(this ICollection<Usuario> usuarios, CoeusProjectContext context = null)
        {
            return usuarios.AsQueryable<Usuario>().Decrypt(context);
        }

        public static List<Usuario> Decrypt(this IEnumerable<Usuario> usuarios, CoeusProjectContext context = null)
        {
            return usuarios.AsQueryable<Usuario>().Decrypt(context);
        }

        public static List<Usuario> Encrypt(this IQueryable<Usuario> usuarios)
        {
            List<Usuario> usuariosRet = new List<Usuario>();
            foreach(Usuario usuario in usuarios)
            {
                usuariosRet.Add(usuario.Encrypt());
            }
            return usuariosRet;
        }

        public static List<Usuario> Encrypt (this ICollection<Usuario > usuarios)
        {
            return usuarios.AsQueryable<Usuario>().Encrypt();
        }

        public static List<Artigo> Decrypt(this IQueryable<Artigo> artigos)
        {
            List<Artigo> artigosRet = new List<Artigo>();
            foreach (Artigo artigo in artigos)
            {
                artigosRet.Add(artigo.Decrypt());
            }
            return artigosRet;
        }

        public static List<Video> Decrypt(this IQueryable<Video> videos)
        {
            List<Video> videosRet = new List<Video>();
            foreach (Video video in videos)
            {
                videosRet.Add(video.Decrypt());
            }
            return videosRet;
        }

        public static List<Grupo> Decrypt(this IQueryable<Grupo> grupos)
        {
            List<Grupo> gruposRet = new List<Grupo>();
            foreach (Grupo grupo in grupos)
            {
                gruposRet.Add(grupo.Decrypt());
            }
            return gruposRet;
        }

        public static List<Objeto> Decrypt(this IQueryable<Objeto> objetos)
        {
            CoeusProjectContext _context = new CoeusProjectContext();
            List<Objeto> objetosRet = new List<Objeto>();

            foreach (Objeto objeto in objetos)
            {
                objetosRet.Add(objeto.Decrypt(_context));
            }

            return objetosRet;
        }
    }
}