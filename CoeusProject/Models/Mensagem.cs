using CoeusProject.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
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

        public Mensagem Encrypt(Int32 IdGrupo = 0, CoeusProjectContext Context = null)
        {
            if (Context == null)
            {
                Context = new CoeusProjectContext();
            }

            if (IdGrupo > 0)
            {
                this.Grupo = Context.Grupos.Where(g => g.IdGrupo == IdGrupo).FirstOrDefault();
            }

            if (this.Grupo == null)
            {
                this.Grupo = Context.Grupos.Where(g => g.IdGrupo == this.IdGrupo).Include(g=>g.Salt).FirstOrDefault();
            }

            if (this.Grupo.Salt == null)
            {
                this.Grupo.Salt = Context.Salt.Where(s => s.IdSalt == this.Grupo.IdSalt).FirstOrDefault();
            }

            this.TxMensagem = SecurityFacade.Encrypt(this.TxMensagem, this.Grupo.Salt.BtSalt);
            return this;
        }

        public Mensagem Decrypt(Int32 IdGrupo = 0, Usuario usuario = null, CoeusProjectContext Context = null)
        {
            if (IdGrupo > 0)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Grupo = Context.Grupos.Where(g=>g.IdGrupo == IdGrupo).FirstOrDefault();
            }

            if (this.Grupo == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Grupo = Context.Grupos.Where(g => g.IdGrupo == this.IdGrupo).Include(g => g.Salt).FirstOrDefault();
            }

            if (this.Grupo.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Grupo.Salt = Context.Salt.Where(s => s.IdSalt == this.Grupo.IdSalt).FirstOrDefault();
            }

            this.TxMensagem = SecurityFacade.Decrypt(this.TxMensagem, this.Grupo.Salt.BtSalt);

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