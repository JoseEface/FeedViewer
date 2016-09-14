using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedViewer.Models
{
    public class RecoverPassword
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "Entre com um e-mail válido")]
        public string email {
            get;
            set;
        }
    }
}
