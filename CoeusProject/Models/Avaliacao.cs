using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Avaliacao
    {
        [Key]
        public Int32 IdAvaliacao { get; set; }

        [Required(ErrorMessage = "A nota da avaliação é obrigatória")]
        public Int32 NoAvaliacao { get; set; }

        [ForeignKey("Usuario")]
        public Int32? IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Objeto")]
        public Int32 IdObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }
    }
}