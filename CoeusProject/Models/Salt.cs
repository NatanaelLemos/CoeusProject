using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class Salt
    {
        [Key]
        public Int32 IdSalt { get; set; }

        [Required]
        public String TxSalt { get; set; }

        [NotMapped]
        public byte[] BtSalt
        {
            get
            {
                return GetBytes(this.TxSalt);
            }
        }

        /// <summary>
        /// 0 = Create new Salt
        /// > 0 = Get Salt By Id
        /// </summary>
        public static Salt GetSalt(Int32 IdSalt = 0, CoeusProjectContext Context = null)
        {
            Salt salt = null;
            Boolean hasContext = true;

            if (Context == null)
            {
                hasContext = false;
                Context = new CoeusProjectContext();
            }

            if (IdSalt == 0)
            {
                byte[] saltArray = new byte[16];
                Random rand = new Random();
                salt = new Salt();
                
                for (int i = 0; i < 16; i++)
                {
                    saltArray[i] = (byte)rand.Next(0, 254);
                }

                salt.TxSalt = GetString(saltArray);
                Context.Salt.Add(salt);

                if (!hasContext)
                {
                    Context.SaveChanges();
                }
            }
            else
            {
                salt = Context.Salt.Where(s=>s.IdSalt == IdSalt).FirstOrDefault();
            }
            return salt;
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}