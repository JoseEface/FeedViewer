using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedViewer.Models
{
    public class LoginInfo
    {
        private usuario usuarioLogado;
        private DateTime horaLogon;

        public LoginInfo()
        {
            Usuario = new usuario();
            HoraLogon = DateTime.Now;
        }

        public LoginInfo(usuario usr)
        {
            Usuario = usr;
            HoraLogon = DateTime.Now;
        }

        public usuario Usuario {
            get {
                return this.usuarioLogado;
            }
            set
            {
                this.usuarioLogado = value;
            }
        }

        public DateTime HoraLogon
        {
            get
            {
                return this.horaLogon;
            }
            private set
            {
                this.horaLogon = value;
            }
        }

    }
}