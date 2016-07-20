using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace FeedViewer.Controllers.Autenticacao
{
    public class SemCacheNovo : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            actionContext.Request.Headers.Add("Cache-Control", "max-age=0, no-cache, no-store, must-revalidate");
            actionContext.Request.Headers.Add("Pragma", "no-cache");
            actionContext.Response.Headers.Add("Pragma", "no-cache");
            actionContext.Response.Headers.Add("Cache-Control", "max-age=0, no-cache, no-store, must-revalidate");
            base.OnActionExecuting(actionContext);
        }
    }
}