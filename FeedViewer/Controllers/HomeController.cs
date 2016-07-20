using FeedViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedViewer.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/        

        public ActionResult Index()
        {
            LoginInfo linfo = new LoginInfo(new usuario());
            long qtd,totalSeguido;
            try
            {
                linfo = Session["UsuarioLogado"] as LoginInfo;
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    qtd = contexto.feedrsses.Where(f => f.dono.Equals(linfo.Usuario.id)).LongCount();
                    totalSeguido = contexto.usuariorsses.Where(t => t.idusuario.Equals(linfo.Usuario.id)).LongCount();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Do controller Home: " + e.Message);
                qtd = 0;
                totalSeguido = 0;
            }
            ViewBag.qtdCadastrei = qtd;
            ViewBag.qtdSeguindo = totalSeguido;
            return View(linfo);
        }

        [HttpPost]
        public JsonResult pegaPagina(int pagina, int maxnumero, string filtrado="")
        {
            StatusAJAX resultado = new StatusAJAX();
            List<feedrss> lista = new List<feedrss>();
            long qtd;
            JsonResult jsresultado=null;
            try
            {
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    LoginInfo linfo = Session["UsuarioLogado"] as LoginInfo;
                    IQueryable<usuariorss> consulta = Enumerable.Empty<usuariorss>().AsQueryable();
                    if (filtrado != "" && filtrado != null && filtrado.Trim().Length != 0)
                        consulta = contexto.usuariorsses.Where(u => (u.idusuario.Equals(linfo.Usuario.id) && u.feedrss.nomereferencia.Contains(filtrado)));
                    else
                        consulta = contexto.usuariorsses.Where(u => u.idusuario.Equals(linfo.Usuario.id));

                    qtd = consulta.LongCount();
                    if (qtd != 0)
                    {
                        if (pagina == 1)
                            jsresultado = Json(consulta.
                                            Take(maxnumero).Select(c => new
                                            {
                                                id = c.feedrss.id,
                                                caminho = c.feedrss.urlfeed,
                                                nome = c.feedrss.nomereferencia,
                                                usando = c.feedrss.usuariorsses.LongCount()
                                            }).ToList());
                        else
                        {
                            int minbase = pagina * maxnumero - maxnumero;
                            jsresultado = Json(lista);
                            if (minbase < qtd)
                            {
                                decimal inicio=consulta.Take(minbase).Last().idusuario;
                                jsresultado = Json(consulta.Where(u => u.idfeed > inicio)
                                    .Select(c => c.feedrss).Take(maxnumero)
                                    .Select(c => new
                                    {
                                        id = c.id,
                                        caminho = c.urlfeed,
                                        nome = c.nomereferencia,
                                        usando = c.usuariorsses.LongCount()
                                    }).ToList());
                            }
                        }
                    }
                    if(jsresultado == null ) jsresultado = Json(lista);
                    
                    return jsresultado;
                }
            }
            catch (Exception e)
            {
                resultado.Mensagem = e.Message;
                resultado.Sucesso = false;
            }
            return Json(resultado);
        }

        public long contaTotal(string filtrado="")
        {
            long resultado=0;
            try
            {
                LoginInfo linfo=Session["UsuarioLogado"] as LoginInfo;
                using(feedviewerContext contexto=new feedviewerContext())
                {
                    if (filtrado != null && filtrado != "" && filtrado.Trim().Length != 0)
                        resultado = contexto.usuariorsses.Where(u =>( u.idusuario.Equals(linfo.Usuario.id) && u.feedrss.nomereferencia.Contains(filtrado) )).LongCount();
                    else
                        resultado = contexto.usuariorsses.Where(u => u.idusuario.Equals(linfo.Usuario.id)).LongCount();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write("Do Home-contaTotal: "+e.Message);
                resultado = 0;
            }
            return resultado;
        }


    }
}
