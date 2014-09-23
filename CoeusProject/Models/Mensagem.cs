using CoeusProject.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Mensagem
    {
        [Key]
        public Int32 IdMensagem { get; set; }

        [Required(ErrorMessage="O texto da mensagem é obrigatório")]
        [StringLength(4096, MinimumLength=1, ErrorMessage="Uma mensagem deve conter entre 1 e 1024 caracteres")]
        public String TxMensagem { get; set; }

        public DateTime DtMensagem { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public Int32 IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required]
        [ForeignKey("Grupo")]
        public Int32 IdGrupo { get; set; }
        public virtual Grupo Grupo { get; set; }

        public Mensagem Encrypt()
        {
            this.TxMensagem = SecurityFacade.Encrypt(this.TxMensagem);
            return this;
        }

        public Mensagem Decrypt(Usuario usuario = null)
        {
            this.TxMensagem = SecurityFacade.Decrypt(this.TxMensagem);

            if (usuario != null)
            {
                this.Usuario = usuario;
            }
            else if (this.Usuario != null)
            {
                this.Usuario.Decrypt();
            }

            return this;
        }
    }
}