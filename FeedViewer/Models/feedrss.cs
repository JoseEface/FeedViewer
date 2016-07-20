using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FeedViewer.Models
{
    public partial class feedrss
    {
        public feedrss()
        {
            this.usuariorsses = new List<usuariorss>();
        }

        [Required]
        [Display(Name="id")]
        public decimal id { get; set; }

        [Required(ErrorMessage="Campo obrigat�rio")]
        [Display(Name="Descri��o")]
        public string nomereferencia { get; set; }
        
        [Required(ErrorMessage="Campo obrigat�rio")]
        [Url(ErrorMessage="Tem que ser uma URL v�lida")]
        [Display(Name="URL do Feed")]
        public string urlfeed { get; set; }
        
        [Required(ErrorMessage="Campo obrigat�rio")]
        [Display(Name="Quem cadastrou")]
        public decimal dono { get; set; }
        
        public virtual ICollection<usuariorss> usuariorsses { get; set; }
        public virtual usuario usuario { get; set; }
    }
}
