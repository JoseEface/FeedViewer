using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedViewer.Controllers.Autenticacao
{
    public class NoCache:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            filterContext.HttpContext.Request.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            filterContext.HttpContext.Request.Headers.Add("Expires", "0");            
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            filterContext.HttpContext.Response.Cache.SetNoStore();
            filterContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
            filterContext.HttpContext.Response.Cache.SetProxyMaxAge(TimeSpan.Zero);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches); //IMPORTANTE
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false); //IMPPORTANTE
            System.Diagnostics.Debug.WriteLine("AQUI NO FILTRO");
        }
    }
}