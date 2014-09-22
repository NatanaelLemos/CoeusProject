using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Tema
    {
        [Key]
        public Int32 IdTema { get; set; }

        [Required(ErrorMessage = "A descrição do tema é obrigatória")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "A descrição do tema deve conter entre 1 e 20 caracteres")]
        public String NmTema { get; set; }

        public virtual IEnumerable<Objeto> Objetos { get; set; }

        public virtual IEnumerable<Usuario> Usuarios { get; set; }
    }
}