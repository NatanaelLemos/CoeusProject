using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Objeto
    {
        [Key]
        public Int32 IdObjeto { get; set; }

        [Required(ErrorMessage = "O nome do objeto é obrigatório")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O nome do objeto deve conter entre 1 e 50 caracteres")]
        public String NmObjeto { get; set; }

        [StringLength(2048, ErrorMessage = "A descrição deve conter no máximo 512 caracteres")]
        public String TxDescricao { get; set; }

        [ForeignKey("Usuario")]
        public Int32 IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        /*[ForeignKey("Video")]
        public Int32? IdVideo { get; set; }
        public virtual Video Video { get; set; }

        [ForeignKey("Som")]
        public Int32? IdSom { get; set; }
        public virtual Som Som { get; set; }

        [ForeignKey("Artigo")]
        public Int32? IdArtigo { get; set; }
        public virtual Artigo Artigo { get; set; }
        */
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; }

        public virtual ICollection<Tema> Temas { get; set; }
    }
}