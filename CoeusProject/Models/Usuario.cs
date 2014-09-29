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
    public class Usuario
    {
        #region Dados do usuario
        [Key]
        public Int32 IdUsuario { get; set; }

        [Display(Name = "E-Mail")]
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [StringLength(1024, MinimumLength = 5, ErrorMessage = "O E-Mail deve conter entre 5 e 255 caracteres")]
        [EmailAddress(ErrorMessage = "E-Mail inválido")]
        public String TxEmail { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(1024, MinimumLength = 6, ErrorMessage = "A senha deve conter entre 6 e 255 caracteres")]
        public String PwUsuario { get; set; }
        #endregion

        #region Dados da pessoa
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(1024, MinimumLength = 1, ErrorMessage = "O nome deve conter entre 1 e 255 caracteres")]
        public String NmPessoa { get; set; }

        [Display(Name = "Sobrenome")]
        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(1024, MinimumLength = 1, ErrorMessage = "O sobrenome deve conter entre 1 e 255 caracteres")]
        public String SnPessoa { get; set; }

        [DisplayName("Foto")]
        [StringLength(100)]
        public String NmFoto { get; set; }

        [NotMapped]
        public string NmThumbFoto 
        { 
            get 
            {
                return NmFoto.Replace(".png", "-thumb.png");
            } 
        }
        #endregion

        public virtual ICollection<Grupo> Grupos { get; set; }

        public virtual ICollection<Mensagem> Mensagens { get; set; }

        public virtual ICollection<Objeto> Objetos { get; set; }

        public virtual ICollection<Avaliacao> Avaliacoes { get; set; }

        public virtual ICollection<Tema> Temas { get; set; }

        [ForeignKey("Salt")]
        public Int32 IdSalt { get; set; }
        public virtual Salt Salt { get; set; }

        public Usuario Encrypt(CoeusProjectContext Context = null)
        {
            if (this.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }

                this.Salt = Context.Salt.Where(s=>s.IdSalt == this.IdSalt).FirstOrDefault();
            }

            this.TxEmail = SecurityFacade.Encrypt(TxEmail, this.Salt.BtSalt) + "_@email.com";
            this.PwUsuario = SecurityFacade.Encrypt(PwUsuario, this.Salt.BtSalt);
            this.NmPessoa = SecurityFacade.Encrypt(NmPessoa, this.Salt.BtSalt);
            this.SnPessoa = SecurityFacade.Encrypt(SnPessoa, this.Salt.BtSalt);

            return this;
        }

        public Usuario Decrypt(CoeusProjectContext Context = null)
        {
            if (this.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }

                this.Salt = Context.Salt.Where(s => s.IdSalt == this.IdSalt).FirstOrDefault();
            }

            this.TxEmail = SecurityFacade.Decrypt(TxEmail.Replace("_@email.com", ""), this.Salt.BtSalt);
            this.NmPessoa = SecurityFacade.Decrypt(NmPessoa, this.Salt.BtSalt);
            this.SnPessoa = SecurityFacade.Decrypt(SnPessoa, this.Salt.BtSalt);
            
            return this;
        }
    }
}