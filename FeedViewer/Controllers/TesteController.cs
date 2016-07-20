using FeedViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedViewer.Controllers
{
    public class TesteController : Controller
    {
        //
        // GET: /Teste/
        [HttpGet]
        public ActionResult Index()
        {   
            return View(new usuario());
        }

        [HttpPost]
        public JsonResult Index(usuario u)
        {
            return Json(u);
        }
    }
}
