using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace CoeusProject.Models
{
    public class CoeusProjectContext : DbContext
    {
//#if DEBUG
//        public CoeusProjectContext() : base("CoeusProjectContext")
//        {
//        }
//#else
        public CoeusProjectContext() : base("CoeusPublish")
        {
        }
//#endif

        public DbSet<Artigo> Artigos { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Imagem> Imagens { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<Objeto> Objetos { get; set; }
        public DbSet<Som> Sons { get; set; }
        public DbSet<Tema> Temas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Sequence> Sequence { get; set; }
        public DbSet<Salt> Salt { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}
