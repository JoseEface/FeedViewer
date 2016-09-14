using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FeedViewer.Models.Mapping;

namespace FeedViewer.Models
{
    public partial class feedviewerContext : DbContext
    {
        static feedviewerContext()
        {
            Database.SetInitializer<feedviewerContext>(null);
        }

        public feedviewerContext()
            : base("Name=feedviewerContext")
        {
        }

        public DbSet<feedrss> feedrsses { get; set; }
        public DbSet<usuario> usuarios { get; set; }
        public DbSet<usuariorss> usuariorsses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new feedrssMap());
            modelBuilder.Configurations.Add(new usuarioMap());
            modelBuilder.Configurations.Add(new usuariorssMap());
        }
    }
}
