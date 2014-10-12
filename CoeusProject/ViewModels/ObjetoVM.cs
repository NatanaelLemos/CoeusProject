using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoeusProject.Models;
using System.Data.Entity;

namespace CoeusProject.ViewModels
{
    public class ObjetoVM
    {
        public ObjetoVM(Objeto objeto, CoeusProjectContext context = null)
        {
            Avaliacoes = objeto.Avaliacoes;
            IdObjeto = objeto.IdObjeto;
            IdUsuario = objeto.IdUsuario;
            NmObjeto = objeto.NmObjeto;
            Temas = objeto.Temas;
            TxDescricao = objeto.TxDescricao;
            Usuario = objeto.Usuario;

            if (context == null)
            {
                context = new CoeusProjectContext();
            }

            Video video = context.Videos.Where(v => v.IdObjeto == objeto.IdObjeto).FirstOrDefault();
            if (video == null)
            {
                Artigo artigo = context.Artigos.Where(v => v.IdObjeto == objeto.IdObjeto).FirstOrDefault();
                IdArtigo = artigo.IdArtigo;
                TpObjeto = TipoObjeto.Artigo;
            }
            else
            {
                IdVideo = video.IdVideo;
                TpObjeto = TipoObjeto.Video;
            }
        }

        public Int32 IdObjeto { get; set; }
        public String NmObjeto { get; set; }
        public String TxDescricao { get; set; }
        public Int32 IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<Avaliacao> Avaliacoes { get; set; }
        public virtual ICollection<Tema> Temas { get; set; }

        public TipoObjeto TpObjeto { get; set; }

        public Int32 IdVideo { get; set; }
        public Int32 IdArtigo { get; set; }
    }

    public enum TipoObjeto : int
    {
        Artigo = 1,
        Video = 2
    }
}