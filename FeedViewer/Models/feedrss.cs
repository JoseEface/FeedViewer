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

        [Required(ErrorMessage="Campo obrigatório")]
        [Display(Name="Descrição")]
        public string nomereferencia { get; set; }
        
        [Required(ErrorMessage="Campo obrigatório")]
        [Url(ErrorMessage="Tem que ser uma URL válida")]
        [Display(Name="URL do Feed")]
        public string urlfeed { get; set; }
        
        [Required(ErrorMessage="Campo obrigatório")]
        [Display(Name="Quem cadastrou")]
        public decimal dono { get; set; }
        
        public virtual ICollection<usuariorss> usuariorsses { get; set; }
        public virtual usuario usuario { get; set; }
    }
}
