using FeedViewer.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedViewer.Controllers
{
    public class RelatorioController : Controller
    {
        //
        // GET: /Relatorio/

        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public FileResult RetornaArquivo()
        {
            ReportViewer rv=new ReportViewer();
            LoginInfo linfo=Session["UsuarioLogado"] as LoginInfo;            
            string mimeType,encoding,extensao;
            string []lista;
            Warning []listaW;
            byte[] conteudo;

            using(feedviewerContext contexto=new feedviewerContext())
            {   
                ReportDataSource rds=new ReportDataSource("dsPersonalizado", contexto.usuariorsses.Where(u => u.usuario.id.Equals(linfo.Usuario.id)).Select(c => new { nomefeed = c.feedrss.nomereferencia, dono = c.feedrss.usuario.nome, qtdusando = c.feedrss.usuariorsses.LongCount() }));
                rv.LocalReport.Refresh();
                FileStream arquivo = System.IO.File.Open(Server.MapPath("~/Controllers/Relatorio_RSSFeed.rdlc"), FileMode.Open); 
                rv.LocalReport.LoadReportDefinition(arquivo);                
                rv.LocalReport.DataSources.Clear();
                rv.LocalReport.DataSources.Add(rds);
                conteudo=rv.LocalReport.Render("PDF",null,out mimeType, out encoding, out extensao,out lista,out listaW);
                arquivo.Close();
            }

            return File(conteudo,mimeType,"RlFeeds.pdf");
        }

    }
}
