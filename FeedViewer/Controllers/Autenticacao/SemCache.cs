using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FeedViewer.Controllers.Autenticacao
{
    public class SemCache:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            filterContext.HttpContext.Response.Cache.SetNoStore();
            filterContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
            filterContext.HttpContext.Response.Cache.SetProxyMaxAge(TimeSpan.Zero);

            filterContext.RequestContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.RequestContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            filterContext.RequestContext.HttpContext.Response.Cache.SetNoStore();
            filterContext.RequestContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
            filterContext.RequestContext.HttpContext.Response.Cache.SetProxyMaxAge(TimeSpan.Zero);            
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            filterContext.HttpContext.Response.Cache.SetNoStore();
            filterContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
            filterContext.HttpContext.Response.Cache.SetProxyMaxAge(TimeSpan.Zero);            

            filterContext.RequestContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.RequestContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            filterContext.RequestContext.HttpContext.Response.Cache.SetNoStore();
            filterContext.RequestContext.HttpContext.Response.Cache.SetMaxAge(TimeSpan.Zero);
            filterContext.RequestContext.HttpContext.Response.Cache.SetProxyMaxAge(TimeSpan.Zero);            
            base.OnResultExecuting(filterContext);
        }
    }
}