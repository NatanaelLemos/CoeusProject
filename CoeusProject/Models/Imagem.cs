using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Imagem
    {
        [Key]
        public Int32 IdImagem { get; set; }

        [Required]
        [StringLength(4096)]
        public String TxUrl { get; set; }

        [Required]
        [ForeignKey("Artigo")]
        public Int32 IdArtigo { get; set; }
        public virtual Artigo Artigo { get; set; }
    }
}