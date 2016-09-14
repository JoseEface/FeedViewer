using System;
using System.Collections.Generic;

namespace FeedViewer.Models
{
    public partial class usuariorss
    {
        public decimal idusuario { get; set; }
        public decimal idfeed { get; set; }
        public System.DateTime ultimadata { get; set; }
        public virtual feedrss feedrss { get; set; }
        public virtual usuario usuario { get; set; }
    }
}
