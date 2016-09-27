using FeedViewer.Controllers.Patch;
using FeedViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;


namespace FeedViewer.Controllers
{
    public class RSSFeedController : Controller,IDisposable
    {
        //feedviewerContext contexto;

        public RSSFeedController()
        {
            //contexto = new feedviewerContext();  
        }

        //
        // GET: /RSSFeed/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Teste()
        {
            return View(new feedrss());
        }

        [HttpPost]
        public ActionResult Teste(feedrss valores)
        {
            if (ModelState.IsValid)
            {

            }
            return View(new feedrss());
        }


        [HttpGet]
        [Authorize(Roles="admin")]
        public ActionResult Cadastro(string id="0")
        {
            //feedrss rssatual=new feedrss();
            /*try
            {

                if (id != "0")
                {
                    decimal idfeed = Convert.ToDecimal(id);
                    rssatual = contexto.feedrsses.Where(f => f.id.Equals(idfeed)).Single<feedrss>();
                }
                else rssatual.id = 0;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("RSSFeed/Cadastro:: Feed opcional nao especficiado OK - "+e.Message);
                RedirectToAction("Cadastro","RSSFeed",new {id=""});
            }*/
            return View(/*rssatual*/);
        }

        [HttpPost]
        public JsonResult CarregaRSS(decimal id)
        {
            JsonResult resultado=null;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                resultado=Json(contexto.feedrsses.Where(f => f.id.Equals(id)).Select(c => new
                {
                    id = c.id,
                    iddono = c.dono,
                    nome = c.nomereferencia,
                    caminhofeed = c.urlfeed,
                    usrDono = c.usuario.nome
                }).Single());
            }
            return resultado;
        }

        [HttpPost]
        public JsonResult BuscaRSS()
        {
            JsonResult resultado = null;
            StatusAJAX estado=new StatusAJAX();
            try
            {
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    IQueryable<feedrss> consulta = Enumerable.Empty<feedrss>().AsQueryable();
                    if (Request["rssbusca"] != null && Request["rssbusca"].Length != 0)
                    {
                        string buscado = Request["rssbusca"];
                        consulta = contexto.feedrsses.Where(f => f.nomereferencia.Contains(buscado));
                    }
                    /*if (Request["donofeedbusca"] != null && Request["donofeedbusca"].Length != 0)
                    {
                        decimal iddono=Convert.ToDecimal(Request["donofeedbusca"]);
                        consulta = contexto.feedrsses.Where(f => f.dono.Equals(iddono));
                    }*/
                    resultado = Json(consulta.Select(c => new
                    {
                        id = c.id,
                        iddono = c.dono,
                        nome = c.nomereferencia,
                        caminhofeed = c.urlfeed,
                        usrDono = c.usuario.nome,
                        usando = c.usuariorsses.Count
                    }).ToList());
                }
                return resultado;
            }
            catch (Exception e)
            {
                estado.Sucesso = false;
                estado.Mensagem = e.Message;
            }

            return Json(estado);
        }

        /***
         * TODO
         * 
         * Verficar classe para autenticação de usuario e substituir as usuario pela nova classe
         ***/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GravarCadastro()
        {
            feedrss novoFeed=new feedrss();
            StatusAJAX estado = new StatusAJAX();
            try
            {
                decimal id = Convert.ToDecimal(Request["id"]);
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    if (id != 0)
                        novoFeed = contexto.feedrsses.Where(f => f.id.Equals(id)).Single<feedrss>();
                    novoFeed.nomereferencia = Request["nomefeed"];
                    novoFeed.urlfeed = Request["caminhorss"];                    
                    if (id == 0 && User.Identity.IsAuthenticated)
                        novoFeed.dono = (Session["UsuarioLogado"] as LoginInfo).Usuario.id;
                    if (novoFeed.id != 0)
                        contexto.SaveChanges();
                    else
                    {
                        contexto.feedrsses.Add(novoFeed);
                        contexto.SaveChanges();
                    }
                }
                estado.Sucesso = true;
            }
            catch (Exception e)
            {
                estado.Mensagem = e.Message;
                estado.Sucesso = false;
            }
            return Json(estado);
        }

        [HttpPost]
        public JsonResult Remover(decimal id)
        {
            StatusAJAX resultado = new StatusAJAX();

            using (feedviewerContext contexto = new feedviewerContext())
            {
                try
                {
                    LoginInfo linfo = Session["UsuarioLogado"] as LoginInfo;
                    if (linfo.Usuario.admin || possoEditar(id, linfo.Usuario.id))
                    {
                        feedrss seraremovido = contexto.feedrsses.Where(f => f.id.Equals(id)).Single<feedrss>();
                        contexto.feedrsses.Remove(seraremovido);
                        contexto.SaveChanges();
                    }
                    else
                        throw new Exception("Você não tem autorização para remoção");
                    resultado.Sucesso = true;
                }
                catch (Exception e)
                {
                    resultado.Mensagem = e.Message;
                    resultado.Sucesso = false;
                }
            }
            return Json(resultado);
        }

        public bool tenhoAdicionado(decimal idfeed, decimal iddono)
        {
            bool resultado=false;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                resultado=(contexto.usuariorsses.Where(u => (u.idfeed.Equals(idfeed) && u.idusuario.Equals(iddono))).Count()>0);
            }
            return resultado;
        }

        public bool possoEditar(decimal idfeed, decimal iddono)
        {
            bool resultado = false;
            long quantidade=0;
            using (feedviewerContext contexto = new feedviewerContext())
            {
                if (contexto.usuarios.Where(u => u.id.Equals(iddono)).Single<usuario>().admin) ///admin tem poder absoluto
                    resultado = true;
                else
                {

                    quantidade = contexto.usuariorsses.Where(u => u.idfeed.Equals(idfeed)).LongCount();
                    if (quantidade > 1) ///Mais de um está usando
                    {
                        resultado = false;
                    }
                    else if (quantidade == 0) ///Niguem está usando, ele deve ser o dono
                    {
                        long valor = contexto.feedrsses.Where(f => (f.id.Equals(idfeed) && f.dono.Equals(iddono))).LongCount();
                        if (valor != 0)
                            resultado = true;
                    }
                    else /// = 1, verifica se quem está usando é o dono
                    {
                        long valor = contexto.usuariorsses.Where(u => (u.idfeed.Equals(idfeed) && u.idusuario.Equals(iddono) 
                                                                       && u.feedrss.dono.Equals(iddono))).LongCount();
                        if (valor > 0)
                            resultado = true;
                        else
                            resultado = false;
                    }
                }
            }
            return resultado;
        }

        public JsonResult BuscaRssCondicional()
        {
            string rssbuscado=Request["rssbusca"];
            StatusAJAX resultado = new StatusAJAX();
            JsonResult retorno = null;
            //try
            //{
                IQueryable<feedrss> consulta=Enumerable.Empty<feedrss>().AsQueryable();
                int opcao = Convert.ToInt32(Request["rssbuscatipo"]);
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    consulta=contexto.feedrsses.Where(f => f.nomereferencia.Contains(rssbuscado));
                    LoginInfo linfo = Session["UsuarioLogado"] as LoginInfo;
                    bool otimizaEdicao = false;
                    if (opcao == 0) //Não tenho na minha lista
                    {   
                        consulta=consulta.Where(f => (f.usuariorsses.LongCount()==0 || (f.usuariorsses.LongCount()>0 && f.usuariorsses.Where(u => u.idusuario.Equals(linfo.Usuario.id)).LongCount() == 0)));
                    }
                    else if (opcao == 1) //Todos já buscados
                    {
                        /// Sem operacao
                    }
                    else if (opcao == 2) //Sou dono e não tenho adicionado
                    {
                        System.Diagnostics.Debug.WriteLine("DEBUG: " + linfo.Usuario.id);
                        consulta=consulta.Where(f=>(f.dono.Equals(linfo.Usuario.id) && 
                                           (f.usuariorsses.LongCount() == 0 || (f.usuariorsses.LongCount()>0 &&
                                           f.usuariorsses.Where(u=>u.idusuario.Equals(linfo.Usuario.id)).LongCount()==0))) );
                    }
                    else if (opcao == 3) //Sou dono e posso editar
                    {
                        ///Use AsEnumerable para permitir usar metodos internos no linq.... Depois volte ao normal com Queryable
                        consulta=consulta.AsEnumerable().Where(f => (possoEditar(f.id,linfo.Usuario.id) && f.dono.Equals(linfo.Usuario.id))).AsQueryable();
                        otimizaEdicao = true;
                    }
                    else if (opcao == 4) //Que está na minha lista de RSS
                    {
                        consulta=consulta.Where(f => f.usuariorsses.Where(u => u.idusuario.Equals(linfo.Usuario.id)).LongCount() != 0);
                    }
                    if (!otimizaEdicao)
                    {
                        ///Use AsEnumerable para permitir usar metodos internos no linq....
                        retorno = Json(consulta.Select(z => new { qUsando=z.usuariorsses.Count, copia=z}).
                            AsEnumerable().Select(f => new
                                {
                                    id = f.copia.id,
                                    nome = f.copia.nomereferencia,
                                    caminho = f.copia.urlfeed,
                                    dono = f.copia.dono,
                                    qtdusando = f.qUsando,
                                    adicionar = (!tenhoAdicionado(f.copia.id, linfo.Usuario.id)),
                                    remover = tenhoAdicionado(f.copia.id, linfo.Usuario.id),
                                    editar = possoEditar(f.copia.id, linfo.Usuario.id)
                                }).ToList());
                    }
                    else
                    {
                        retorno = Json(consulta.AsEnumerable().Select(z => new { qUsando = z.usuariorsses.Count, copia = z }).AsEnumerable().
                            Select(f => new
                        {
                            id = f.copia.id,
                            nome = f.copia.nomereferencia,
                            caminho = f.copia.urlfeed,
                            dono = f.copia.dono,
                            qtdusando = f.qUsando,
                            adicionar = (!tenhoAdicionado(f.copia.id, linfo.Usuario.id)),
                            remover = tenhoAdicionado(f.copia.id, linfo.Usuario.id),
                            editar = true
                        }).ToList());
                    }
                }
                resultado.Sucesso=true;
                //return Json(retorno);
            //}
           // catch (Exception e)
            //{
            //    resultado.Sucesso = false;
            //    resultado.Mensagem = e.Message;
            //}
            if (resultado.Sucesso)
                return retorno;
            else
                return Json(resultado);
        }

        public JsonResult Detalhes(decimal id, int maxnot)
        {
            JsonResult resultado=null;
            StatusAJAX estado = new StatusAJAX();

            try
            {
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    feedrss rssbuscado = contexto.feedrsses.Where(f => f.id.Equals(id)).Single<feedrss>();
                    var feed = Argotic.Syndication.RssFeed.Create(new Uri(rssbuscado.urlfeed));
                    List<SyndicationItem> listaMovimentacao;

                    if (maxnot == 0)
                        listaMovimentacao = feed.Channel.Items.Select(f => new SyndicationItem() { Summary = new TextSyndicationContent(f.Description), Title = new TextSyndicationContent(f.Title) }).ToList();
                    else
                        listaMovimentacao = feed.Channel.Items.Select(f => new SyndicationItem() { Summary = new TextSyndicationContent(f.Description), Title = new TextSyndicationContent(f.Title) }).Take(maxnot).ToList();

                    resultado = Json(listaMovimentacao);
                    estado.Sucesso = true;
                }
            }
            catch(Exception e)
            {
                estado.Sucesso = false;
                estado.Mensagem = e.Message;
            }

            if (estado.Sucesso) return resultado;

            return Json(estado);
        }

        public JsonResult RemoveDaListaPessoal(decimal id)
        {
            StatusAJAX estado = new StatusAJAX();
            try
            {
                LoginInfo linfo = Session["UsuarioLogado"] as LoginInfo;
                decimal idusuario = linfo.Usuario.id;
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    usuariorss urss=contexto.usuariorsses.Where(u => (u.idusuario.Equals(idusuario) && u.idfeed.Equals(id))).Single();
                    contexto.usuariorsses.Remove(urss);
                    contexto.SaveChanges();
                }
                estado.Sucesso = true;
                estado.Mensagem = "OK";
            }
            catch (Exception e)
            {
                estado.Sucesso = false;
                estado.Mensagem = e.Message;
            }
            return Json(estado);
        }

        public JsonResult PoeNaListaPessoal(decimal id) {
            StatusAJAX estado = new StatusAJAX();
            try
            {
                LoginInfo linfo = Session["UsuarioLogado"] as LoginInfo;
                decimal idusuario = linfo.Usuario.id;
                using (feedviewerContext contexto = new feedviewerContext())
                {
                    usuariorss ur=new usuariorss();
                    ur.idfeed = id;
                    ur.idusuario = idusuario;
                    ur.ultimadata = DateTime.Now;                    
                    contexto.usuariorsses.Add(ur);
                    contexto.SaveChanges();
                }
                estado.Sucesso = true;
                estado.Mensagem = "OK";
            }
            catch (Exception e)
            {
                estado.Sucesso = false;
                estado.Mensagem = e.Message;
            }
            return Json(estado);
        }

        void IDisposable.Dispose()
        {
            //contexto.Dispose();
        }
    }
}
