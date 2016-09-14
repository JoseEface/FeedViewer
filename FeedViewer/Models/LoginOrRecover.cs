using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedViewer.Models
{
    public class LoginOrRecover
    {
         public LoginUsuario loginUsr { get; set; }
         public RecoverPassword recPasswd { get; set; }

         public LoginOrRecover()
         {
            loginUsr=new LoginUsuario();
            recPasswd = new RecoverPassword();
         }
    }
}