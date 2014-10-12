using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CoeusProject.Facade;

namespace CoeusProject.Models
{
    public class Video
    {
        [Key]
        public Int32 IdVideo { get; set; }

        [Required]
        [StringLength(4096)]
        public String TxUrl { get; set; }

        [Required]
        [StringLength(4096)]
        public string TxUrlPoster { get; set; }

        [Required]
        [ForeignKey("Objeto")]
        public Int32 IdObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }

        public Video Encrypt(CoeusProjectContext Context = null)
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
            this.TxUrl = SecurityFacade.Encrypt(this.TxUrl, this.Objeto.Salt.BtSalt);

            return this;
        }

        public Video Decrypt(CoeusProjectContext Context = null)
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
            this.TxUrl = SecurityFacade.Decrypt(this.TxUrl, this.Objeto.Salt.BtSalt);
            return this;
        }

    }
}