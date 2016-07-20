using FeedViewer.Controllers.Autenticacao;
using FeedViewer.Models;
using System;
using System.Collections.Generic;
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

    }
}
