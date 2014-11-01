using CoeusProject.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Objeto
    {
        [Key]
        public Int32 IdObjeto { get; set; }

        [Required(ErrorMessage = "O nome do objeto é obrigatório")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O nome do objeto deve conter entre 1 e 50 caracteres")]
        public String NmObjeto { get; set; }

        [StringLength(2048, ErrorMessage = "A descrição deve conter no máximo 512 caracteres")]
        public String TxDescricao { get; set; }

        [ForeignKey("Usuario")]
        public Int32 IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<Avaliacao> Avaliacoes { get; set; }

        public virtual ICollection<Tema> Temas { get; set; }

        [ForeignKey("Salt")]
        public Int32 IdSalt { get; set; }
        public virtual Salt Salt { get; set; }

        public int QtAcessos { get; set; }

        [NotMapped]
        public Decimal VlMediaAvaliacao
        {
            get 
            {
                if (Avaliacoes == null || Avaliacoes.Count == 0) return 0;

                Int32 idx = 0;
                Int32 vlTotalAvaliacao = 0;
                foreach (Avaliacao av in this.Avaliacoes)
                {
                    vlTotalAvaliacao += av.NoAvaliacao;
                    idx++;
                }

                idx = idx == 0 ? 1 : idx;

                return Math.Round(((decimal)(vlTotalAvaliacao / idx)), 2);
            }
        }

        [NotMapped]
        public Decimal VlMediaAvaliacaoCalc { get; set; }

        public Objeto Encrypt(CoeusProjectContext Context = null)
        {
            if (this.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Salt = Context.Salt.Where(s => s.IdSalt == this.IdSalt).FirstOrDefault();
            }

            this.NmObjeto = SecurityFacade.Encrypt(this.NmObjeto, this.Salt.BtSalt);
            this.TxDescricao = SecurityFacade.Encrypt(this.TxDescricao, this.Salt.BtSalt);
            return this;
        }

        public Objeto Decrypt(CoeusProjectContext Context = null)
        {
            if (this.Salt == null)
            {
                if (Context == null)
                {
                    Context = new CoeusProjectContext();
                }
                this.Salt = Context.Salt.Where(s => s.IdSalt == this.IdSalt).FirstOrDefault();
            }

            this.NmObjeto = SecurityFacade.Decrypt(this.NmObjeto, this.Salt.BtSalt);
            this.TxDescricao = SecurityFacade.Decrypt(this.TxDescricao, this.Salt.BtSalt);
            return this;
        }
    }
}