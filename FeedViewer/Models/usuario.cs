using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedViewer.Models
{
    public partial class usuario
    {
        public usuario()
        {
            this.feedrsses = new List<feedrss>();
            this.usuariorsses = new List<usuariorss>();
            this.admin = false;
        }

        public decimal id { get; set; }
        
        [Required]
        [Display(Name="Nome")]
        public string nome { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name="E-mail")]
        public string email { get; set; }
        
        [Required]
        [Display(Name="Login")]
        public string login { get; set; }
        
        [Required]
        [Display(Name="Senha")]
        public string senha { get; set; }

        [NotMapped]
        [Compare("senha")]
        [Display(Name="Confirmar senha")]
        public string senhaconfirma { get; set; }
        
        [UIHint("MostraSimNao")]
        public bool admin { get; set; }
        
        public virtual ICollection<feedrss> feedrsses { get; set; }
        
        public virtual ICollection<usuariorss> usuariorsses { get; set; }
    }
}
