using FeedViewer.Controllers.Autenticacao;
using FeedViewer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers;
using System.Web.Security;

namespace FeedViewer.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Request.Headers.Add("Pragma", "no-cache");            
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Request.Headers.Add("Expires", "");
            Response.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetMaxAge(TimeSpan.Zero);
            Response.Cache.SetProxyMaxAge(TimeSpan.Zero);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetValidUntilExpires(false);

            return View();
        }

        [HttpGet]
//        [SemCache]
//        [SemCacheNovo]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated || Session.Count != 0)
            {

                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                FormsAuthentication.SignOut();
            }
            //else
            //    return RedirectToAction("Index", "Login");
//            Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
//            Request.Headers.Add("Pragma", "no-cache");
//            Response.Clear();
/**/            
            Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Request.Headers.Add("Expires", "0");            
            Response.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetMaxAge(TimeSpan.Zero);
            Response.Cache.SetProxyMaxAge(TimeSpan.Zero);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches); //IMPORTANTE
            Response.Cache.SetValidUntilExpires(false); //IMPPORTANTE
      
            //return RedirectToAction("EspereLogout", "Login");
            return View();
            //return Redirect("~/");
            //return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public ActionResult EspereLogout()
        {
            //Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            //Request.Headers.Add("Pragma", "no-cache");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetMaxAge(TimeSpan.Zero);
            Response.Cache.SetProxyMaxAge(TimeSpan.Zero);
            return View();
        }

        [HttpPost]
        public JsonResult LogoutTeste()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            //Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            //Request.Headers.Add("Pragma", "no-cache");            
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetMaxAge(TimeSpan.Zero);
            Response.Cache.SetProxyMaxAge(TimeSpan.Zero);
            
            //return RedirectToAction("EspereLogout", "Login");
            return Json(true);
            //return RedirectToAction("Index","Home");
        }


        private bool UsuarioValido(LoginUsuario usuario, out usuario u)
        {
            bool resultado;            
            using (feedviewerContext contexto = new feedviewerContext())
            {
                try
                {
                    u = contexto.usuarios.Where(usr => usr.login.Equals(usuario.meulogin)).Single<usuario>();                    
                    //byte[] lista = Encoding.UTF8.GetBytes(usuario.senha);
                    byte[] lista = usuario.senha.Select(c=>(byte)c).ToArray();
                    byte[] computado=SHA256.Create().ComputeHash(lista);
                    StringBuilder strfinal = new StringBuilder();
                    foreach (byte a in computado)
                        strfinal.Append(a.ToString("x2"));
                    if (u.senha.Equals(strfinal.ToString()))
                        resultado = true;
                    else
                        resultado = false;
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("LoginController: Usuario invalido: {0}",e.Message);
                    u = null;
                    resultado=false;
                }
            }
            return resultado;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginUsuario usuario)
        {
            LoginInfo loginfo;
            usuario usr = new usuario();

            System.Diagnostics.Debug.WriteLine("ESTOU NO LOGIN");
            
            if (ModelState.IsValid)
            {
                if(User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
                if (UsuarioValido(usuario, out usr))
                {
                    FormsAuthentication.SetAuthCookie(usuario.meulogin, false);
                    loginfo = new LoginInfo(usr);                    
                    Session["UsuarioLogado"] = loginfo;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("erroExtra", "Usuário e/ou senha inválido(s)");
                    return View("Index", usuario);
                }
            }
            else
                return View("Index",usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EsqueceSenha(string email)
        {
            EmailAddressAttribute a = new EmailAddressAttribute();
            if(a.IsValid(email))
            {
                /*Envia e-mail*/
                ModelState.AddModelError("adicionalInfo", "Nova senha enviada.");
            }
            else
            {
                ModelState.AddModelError("email", "Seu e-mail é inválido");
            }
            return View("Index");
        }

        public ActionResult TestaLogin()
        {
            LoginOrRecover logrec = new LoginOrRecover();
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View(logrec);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LoginTesta(LoginUsuario usuario)
        {
            StatusAJAX resultado = new StatusAJAX();
            resultado.Sucesso = false;
            resultado.Mensagem = "Usuário e/ou senha inválido(s)";
            LoginInfo loginfo;
            usuario usr = new usuario();

            System.Diagnostics.Debug.WriteLine("ESTOU NO LOGIN");

            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated) { resultado.Sucesso = true; 
                    resultado.Mensagem= Url.Action("Index", "Home");
                }
                else if (UsuarioValido(usuario, out usr))
                {
                    FormsAuthentication.SetAuthCookie(usuario.meulogin, false);
                    loginfo = new LoginInfo(usr);
                    Session["UsuarioLogado"] = loginfo;
                    resultado.Sucesso = true;
                    resultado.Mensagem = Url.Action("Index", "Home");                    
                }
                else
                {
                    resultado.Sucesso = false;
                    resultado.Mensagem = "Usuário e/ou senha inválido(s)";
                }
            }
            return Json(resultado);
        }

        private List<usuario> temEmail(string email)
        {
            List<usuario> lista = new List<usuario>();
            using (feedviewerContext contexto = new feedviewerContext())
            {
                lista=contexto.usuarios.Where(e => e.email.Equals(email)).ToList<usuario>();
            }
            return lista;
        }


        public JsonResult LoginRecupera(RecoverPassword recupera)
        {
            StatusAJAX resultado = new StatusAJAX();
            resultado.Sucesso = false;
            resultado.Mensagem = "E-mail não cadastrado no sistema.";

            if (ModelState.IsValid) {
                List<usuario> lista = temEmail(recupera.email);
                if(lista.LongCount()  != 0)
                {
                    /**TODO enviar email para os usuarios com o email de recuperacao cadastrados no sistema **/
                    resultado.Sucesso = true;
                    resultado.Mensagem = "Todos os e-mails foram enviados com sucesso !";                   
                }
            }

            return Json(resultado);
        }

    }
}
