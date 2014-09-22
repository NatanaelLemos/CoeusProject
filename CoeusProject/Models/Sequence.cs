using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Sequence
    {
        [Key]
        [StringLength(100)]
        public String NmSequence { get; set; }

        [Required]
        public Int32 VlSequence { get; set; }

        public static Int32 GetSequence(String NmSequence, CoeusProjectContext Context = null)
        {
            if (Context == null) Context = new CoeusProjectContext();

            Sequence sequence = Context.Sequence.Where(s => s.NmSequence == NmSequence).FirstOrDefault();

            if (sequence == null)
            {
                sequence = new Sequence();
                sequence.NmSequence = NmSequence;
                sequence.VlSequence = 1;
                Context.Sequence.Add(sequence);
            }
            else
            {
                sequence.VlSequence = sequence.VlSequence + 1;
                Context.Entry(sequence).State = EntityState.Modified;
            }

            Context.SaveChanges();
            return sequence.VlSequence;
        }
    }
}