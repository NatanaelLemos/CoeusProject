using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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
        [ForeignKey("Objeto")]
        public Int32 IdObjeto { get; set; }
        public virtual Objeto Objeto { get; set; }
    }
}