using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FeedViewer.Models.Mapping
{
    public class feedrssMap : EntityTypeConfiguration<feedrss>
    {
        public feedrssMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.nomereferencia)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.urlfeed)
                .IsRequired()
                .HasMaxLength(65535);

            // Table & Column Mappings
            this.ToTable("feedrss", "feedviewer");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.nomereferencia).HasColumnName("nomereferencia");
            this.Property(t => t.urlfeed).HasColumnName("urlfeed");
            this.Property(t => t.dono).HasColumnName("dono");

            // Relationships
            this.HasRequired(t => t.usuario)
                .WithMany(t => t.feedrsses)
                .HasForeignKey(d => d.dono);

        }
    }
}
