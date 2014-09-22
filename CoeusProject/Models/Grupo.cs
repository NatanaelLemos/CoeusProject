using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Grupo
    {
        [Key]
        public Int32 IdGrupo { get; set; }

        [DisplayName("Nome do Grupo")]
        [Required(ErrorMessage = "O nome do grupo é obrigatório")]
        [StringLength(512, MinimumLength=1, ErrorMessage="O nome do grupo deve conter entre 1 e 100 caracteres")]
        public String NmGrupo { get; set; }

        public virtual IEnumerable<Usuario> Usuarios { get; set; }

        public virtual IEnumerable<Mensagem> Mensagens { get; set; }

        [ForeignKey("Objeto")]
        public Int32? NidObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }
    }
}