using CoeusProject.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        #endregion

        public virtual IEnumerable<Grupo> Grupos { get; set; }

        public virtual IEnumerable<Mensagem> Mensagens { get; set; }

        public virtual IEnumerable<Objeto> Objetos { get; set; }

        public virtual IEnumerable<Avaliacao> Avaliacoes { get; set; }

        public virtual IEnumerable<Tema> Temas { get; set; }

        public Usuario Encrypt()
        {
            this.TxEmail = SecurityFacade.Encrypt(TxEmail) + "_@email.com";
            this.PwUsuario = SecurityFacade.Encrypt(PwUsuario);
            this.NmPessoa = SecurityFacade.Encrypt(NmPessoa);
            this.SnPessoa = SecurityFacade.Encrypt(SnPessoa);

            return this;
        }

        public Usuario Decrypt()
        {
            this.TxEmail = SecurityFacade.Decrypt(TxEmail.Replace("_@email.com",""));
            this.NmPessoa = SecurityFacade.Decrypt(NmPessoa);
            this.SnPessoa = SecurityFacade.Decrypt(SnPessoa);
            
            return this;
        }
    }
}