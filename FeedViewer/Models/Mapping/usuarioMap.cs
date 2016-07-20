using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FeedViewer.Models.Mapping
{
    public class usuarioMap : EntityTypeConfiguration<usuario>
    {
        public usuarioMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.email)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.login)
                .IsRequired()
                .HasMaxLength(12);

            this.Property(t => t.senha)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("usuario", "feedviewer");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.login).HasColumnName("login");
            this.Property(t => t.senha).HasColumnName("senha");
            this.Property(t => t.admin).HasColumnName("admin");
        }
    }
}
