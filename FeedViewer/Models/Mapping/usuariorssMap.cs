using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FeedViewer.Models.Mapping
{
    public class usuariorssMap : EntityTypeConfiguration<usuariorss>
    {
        public usuariorssMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idusuario, t.idfeed });

            // Properties
            this.Property(t => t.idusuario)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.idfeed)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("usuariorss", "feedviewer");
            this.Property(t => t.idusuario).HasColumnName("idusuario");
            this.Property(t => t.idfeed).HasColumnName("idfeed");
            this.Property(t => t.ultimadata).HasColumnName("ultimadata");

            // Relationships
            this.HasRequired(t => t.feedrss)
                .WithMany(t => t.usuariorsses)
                .HasForeignKey(d => d.idfeed);
            this.HasRequired(t => t.usuario)
                .WithMany(t => t.usuariorsses)
                .HasForeignKey(d => d.idusuario);

        }
    }
}
