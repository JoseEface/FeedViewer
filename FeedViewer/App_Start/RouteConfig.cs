using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FeedViewer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UsuarioCadastro",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Usuario", action = "Cadastro", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LogoutSistema",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Logout", id = UrlParameter.Optional }
            );

        }
    }
}