using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Artigo
    {
        [Key]
        public Int32 IdArtigo { get; set; }

        [Required(ErrorMessage="O texto do artigo é obrigatório")]
        [StringLength(50000, MinimumLength=1, ErrorMessage="O artigo deve conter entre 1 e 20000 caracteres")]
        public String TxArtigo { get; set; }

        [Required]
        [ForeignKey("Objeto")]
        public Int32 IdObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }

        public virtual IEnumerable<Imagem> Imagens { get; set; }
    }
}