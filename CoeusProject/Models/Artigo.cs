using CoeusProject.Facade;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CoeusProject.Models
{
    public class Artigo
    {
        [Key]
        public Int32 IdArtigo { get; set; }

        [Required(ErrorMessage="O texto do artigo é obrigatório")]
        [StringLength(50000, MinimumLength=1, ErrorMessage="O artigo deve conter entre 1 e 20000 caracteres")]
        public String TxArtigo { get; set; }

        [Required]
        [ForeignKey("Objeto")]
        public Int32 IdObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }

        public Artigo Encrypt(CoeusProjectContext Context = null)
        {
            if (Context == null)
            {
                Context = new CoeusProjectContext();
            }

            if (this.Objeto == null)
            {
                this.Objeto = Context.Objetos.Where(o => o.IdObjeto == this.IdObjeto).Include(o => o.Salt).FirstOrDefault();
            }

            if (this.Objeto.Salt == null)
            {
                this.Objeto.Salt = Context.Salt.Where(s => s.IdSalt == this.Objeto.IdSalt).FirstOrDefault();
            }

            this.Objeto.Encrypt(Context);
            this.TxArtigo = SecurityFacade.Encrypt(this.TxArtigo, this.Objeto.Salt.BtSalt);

            return this;
        }

        public Artigo Decrypt(CoeusProjectContext Context = null)
        {
            if (Context == null)
            {
                Context = new CoeusProjectContext();
            }

            if (this.Objeto == null)
            {
                this.Objeto = Context.Objetos.Where(o => o.IdObjeto == this.IdObjeto).Include(o => o.Salt).FirstOrDefault();
            }

            if (this.Objeto.Salt == null)
            {
                this.Objeto.Salt = Context.Salt.Where(s => s.IdSalt == this.Objeto.IdSalt).FirstOrDefault();
            }

            this.Objeto.Decrypt(Context);
            this.TxArtigo = SecurityFacade.Decrypt(this.TxArtigo, this.Objeto.Salt.BtSalt);
            return this;
        }
    }
}