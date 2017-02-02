using FeedViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedViewer.Controllers
{
    public class UsuarioController : Controller,IDisposable
    {

        //feedviewerContext contexto;
        
        //
        // GET: /Usuario/

        public UsuarioController()
        {
          //  contexto = new feedviewerContext();
        }

        
        public ActionResult Index()
        {
            usuario u=new usuario();
            u.id = 0;
            if (Session["UsuarioLogado"] != null)
            {
                LoginInfo info = Session["UsuarioLogado"] as LoginInfo;
                u = info.Usuario;
            }
            return View(u);
        }

        [HttpGet]
        [Authorize(Roles="admin")]
        public ActionResult Cadastro(string id="0")
        {
            usuario usr=new usuario();
            return View(usr);
        }

        [HttpPost]
        public JsonResult Busca()
        {
            string nomeusr = Request["nomebuscado"];
            string loginusuario = Request["loginbuscado"];
            JsonResult resultado;

            using (feedviewerContext contexto = new feedviewerContext())
            {
                IQueryable<usuario> consulta = null;
                if (nomeusr != null && nomeusr.Length != 0)
                    consulta = contexto.usuarios.Where(u => u.nome.Contains(nomeusr));
                if (loginusuario != null && loginusuario.Length != 0)
                    consulta = consulta.Where(u => u.login.Contains(loginusuario));
                resultado=Json(consulta.Select(u=>new { id=u.id,
                                                 nome=u.nome                                                 
                                                }).ToList());
            }
            return resultado;
        }

        [HttpPost]
        public JsonResult Busca(decimal id)
        {
            JsonResult resultado;
            using (feedviewerContext contexto = new feedviewerContext())
            {

                resultado= Json(contexto.usuarios.Where(u => u.id.Equals(id)).Select(u => new
                {
                    id = u.id,
                    nome = u.nome
                }).Single());
            }
            return resultado;
        }

        [HttpPost]
        public JsonResult Remove(decimal id)
        {
            StatusAJAX resultado = new StatusAJAX();            
            try
            {
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    usuario usr = contexto.usuarios.Where(u => u.id.Equals(id)).Single<usuario>();
                    contexto.usuarios.Remove(usr);
                }
                resultado.Sucesso = true;
            }
            catch (Exception e)
            {
                resultado.Sucesso=false;
                resultado.Mensagem = e.Message;
            }
            return Json(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Gravar()
        {
            StatusAJAX novoStatus=new StatusAJAX();            
            try
            {
                usuario usr = new usuario();
                decimal id = Convert.ToDecimal(Request["id"]);
                if (Request["senha"] != Request["senhaconfirma"])
                    throw new Exception("Senhas não combinam");
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    if (id != 0)
                    {
                        usr = contexto.usuarios.Where(u => u.id.Equals(id)).Single<usuario>();
                        if (Request["contalogin"] != usr.login && !LoginUnicoServer(Request["contalogin"]))
                            throw new Exception("Login não é único");
                    }
                    else
                        usr.id = 0;
                    usr.login = Request["contalogin"];
                    usr.nome = Request["nome"];
                    usr.email = Request["usremail"];
                    //System.Diagnostics.Debug.WriteLine("Login {0} Nome {1} Email {2}", usr.login, usr.nome, usr.email);
                    if (Request["senha"] != null && Request["senha"].Length != 0)
                        usr.senha = usr.senhaconfirma = Request["senha"];
                    else
                        usr.senhaconfirma = usr.senha; //Necessario a Annotation faz validacao no lado do servidor !
                    if (usr.id != 0)
                    {
                        contexto.SaveChanges();
                        if (Session["UsuarioLogado"] != null)
                        {
                            LoginInfo info = Session["UsuarioLogado"] as LoginInfo;
                            info.Usuario = usr;
                            Session["UsuarioLogado"] = info;
                        }
                    }
                    else
                    {
                        contexto.usuarios.Add(usr);
                        contexto.SaveChanges();
                    }
                }
                novoStatus.Sucesso = true;
                novoStatus.Mensagem = "Sucesso";
            }
            catch(Exception e)
            {
                novoStatus.Sucesso=false;
                novoStatus.Mensagem=e.Message;
            }
            return Json(novoStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GravarCadastro()
        {
            StatusAJAX novoStatus = new StatusAJAX();
            try
            {
                usuario usr = new usuario();
                decimal id = Convert.ToDecimal(Request["id"]);
                if (Request["senha"] != Request["senhaconfirma"])
                    throw new Exception("Senhas não combinam");
                using(feedviewerContext contexto= new feedviewerContext())
                {
                if (id != 0)
                {
                    usr = contexto.usuarios.Where(u => u.id.Equals(id)).Single<usuario>();
                    if (usr.admin && Request["isadmin"] == null)
                        usr.admin = false;
                    if (Request["contalogin"] != usr.login && !LoginUnicoServer(Request["contalogin"]))
                        throw new Exception("Login não é único");
                }
                else
                    usr.id = 0;
                usr.login = Request["contalogin"];
                System.Diagnostics.Debug.WriteLine("ANTIGO: "+Request["nome"] + " " + usr.nome);
                usr.nome = Request["nome"];
                System.Diagnostics.Debug.WriteLine("NOVO: " + Request["nome"] + " " + usr.nome);
                usr.email = Request["usremail"];
                if (Request["isadmin"] != null)
                    usr.admin = true;
                if (Request["senha"] != null && Request["senha"].Length != 0)
                    usr.senha = usr.senhaconfirma = Request["senha"];
                else
                    usr.senhaconfirma = usr.senha; //Necessario a Annotation faz validacao no lado do servidor !
                if (usr.id != 0)
                {
                    contexto.SaveChanges();
                    if (Session["UsuarioLogado"] != null)
                    {
                        LoginInfo info = Session["UsuarioLogado"] as LoginInfo;
                        if (info.Usuario.id == usr.id)
                        {
                            info.Usuario = usr;
                            Session["UsuarioLogado"] = info;
                        }

                    }
                }
                else
                {
                    contexto.usuarios.Add(usr);
                    contexto.SaveChanges();
                }
                }
                novoStatus.Sucesso = true;
                novoStatus.Mensagem = "Sucesso";
            }
            catch (Exception e)
            {
                novoStatus.Sucesso = false;
                novoStatus.Mensagem = e.Message;
            }
            return Json(novoStatus);
        }


        [HttpPost]
        public JsonResult LoginUnico(string login)
        {
            StatusAJAX eunico = new StatusAJAX();
            eunico.Sucesso = false;
            eunico.Mensagem = "O login digitado não é único";
            using (feedviewerContext contexto = new feedviewerContext())
            {
                if (contexto.usuarios.Where(u => u.login.Equals(login)).Count() == 0)
                {
                    eunico.Sucesso = true;
                    eunico.Mensagem = "OK o login é unico";
                }
            }
            return Json(eunico);
        }

        private bool LoginUnicoServer(string login)
        {
            bool resultado = false;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                resultado = (contexto.usuarios.Where(u => u.login.Equals(login)).Count() == 0);
            }
            return resultado;
        }

        [HttpPost]
        public JsonResult BuscaUsuario()
        {
            StatusAJAX resultado = new StatusAJAX();
            try
            {
                string nomelogin = Request["loginbusca"];
                JsonResult encontrados;
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    IQueryable<usuario> consulta = contexto.usuarios.Where(u => u.login.Contains(nomelogin));

                    if (Request["nomebusca"] != null && Request["nomebusca"].Length != 0)
                    {
                        string nomebusca = Request["nomebusca"];
                        consulta = consulta.Where(u => u.login.Contains(nomebusca));
                    }

                    encontrados = Json(consulta.Select(a => new {id=a.id,nome=a.nome,login=a.login,email=a.email }).ToList());
                }
                return encontrados;
            }
            catch (Exception e)
            {
               resultado.Sucesso = false;
               resultado.Mensagem = e.Message;
            }
            return Json(resultado);
        }

        [HttpPost]
        public JsonResult CarregaUsuario(decimal id)
        {
            StatusAJAX status=new StatusAJAX();
            JsonResult resultado=null;
            try
            {
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    resultado=Json(contexto.usuarios.Where(usr => usr.id.Equals(id)).
                                    Select(v => new {id=v.id,nome=v.nome,login=v.login,email=v.email,eadmin=v.admin}).Single());
                    
                }
                status.Sucesso = true;
            }
            catch (Exception e)
            {
                status.Sucesso = false;
                status.Mensagem = e.Message;
            }
            if (status.Sucesso) return resultado;
            return Json(status);
        }

        public JsonResult Remover(decimal id)
        {
            StatusAJAX resultado = new StatusAJAX();
            using (feedviewerContext contexto = new feedviewerContext())
            {
                try
                {
                    usuario usr = contexto.usuarios.Where(u => u.id.Equals(id)).Single<usuario>();
                    contexto.usuarios.Remove(usr);
                    contexto.SaveChanges();
                    resultado.Sucesso = true;
                    resultado.Mensagem = "OK";
                }
                catch (Exception e)
                {
                    resultado.Sucesso = false;
                    resultado.Mensagem = e.Message;
                }
            }
            return Json(resultado);
        }
        
        void IDisposable.Dispose()
        {            
            //contexto.Dispose();
        }
    }
}
