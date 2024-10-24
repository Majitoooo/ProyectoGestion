using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_Gestión.ADO;

namespace Proyecto_Gestión.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult BuscarPorNombre(string nombre, int cantidad)
        {
            try
            {
                ProyectoBusquedaEntities contexto = new ProyectoBusquedaEntities();
                var lista  = contexto.buscarPorNombre(nombre, cantidad);
                return Json(lista,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) 
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}