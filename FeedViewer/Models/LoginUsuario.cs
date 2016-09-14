using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FeedViewer.Models
{
    public class LoginUsuario
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Login")]
        [MaxLength(12, ErrorMessage = "Maximo 12 caracteres")]
        public string meulogin
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Senha")]
        public string senha
        {
            get;
            set;
        }

    }
}