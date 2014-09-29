using CoeusProject.Facade;
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
        [StringLength(512, MinimumLength = 1, ErrorMessage = "O nome do grupo deve conter entre 1 e 100 caracteres")]
        public String NmGrupo { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }

        public virtual ICollection<Mensagem> Mensagens { get; set; }

        [ForeignKey("Objeto")]
        public Int32? IdObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }

        [ForeignKey("Salt")]
        public Int32 IdSalt { get; set; }
        public virtual Salt Salt { get; set; }

        public Grupo Encrypt(CoeusProjectContext Context = null)
        {
            if (this.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Salt = Context.Salt.Where(s => s.IdSalt == this.IdSalt).FirstOrDefault();
            }

            this.NmGrupo = SecurityFacade.Encrypt(this.NmGrupo, this.Salt.BtSalt);
            return this;
        }

        public Grupo Decrypt(CoeusProjectContext Context = null)
        {
            if (this.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Salt = Context.Salt.Where(s => s.IdSalt == this.IdSalt).FirstOrDefault();
            }

            this.NmGrupo = SecurityFacade.Decrypt(this.NmGrupo, this.Salt.BtSalt);
            return this;
        }
    }
}