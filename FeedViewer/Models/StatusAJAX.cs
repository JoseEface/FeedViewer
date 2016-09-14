using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedViewer.Models
{
    public class StatusAJAX
    {
        private bool sucesso;
        private string mensagem;

        public StatusAJAX(bool _sucesso = true, string msg = "")
        {
            if (!_sucesso || msg.Length != 0)
            {
                Sucesso = _sucesso;
                Mensagem = msg;
            }
        }

        public bool Sucesso {
            get {
                return this.sucesso;
            }
            set
            {
                this.sucesso = value;
            }
        }

        public string Mensagem
        {
            get
            {
                return this.mensagem;
            }
            set
            {
                this.mensagem = value;
            }
        }

    }
}