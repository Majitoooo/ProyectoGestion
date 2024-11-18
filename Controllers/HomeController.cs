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

        public JsonResult BuscarPorNombre(string nombre, int cantidad, string tipo = null, string tipo_precio = null)
        {
            try
            {
                ProyectoBusquedaEntities contexto = new ProyectoBusquedaEntities();
                var resultados = contexto.buscarPorNombre(nombre, cantidad, tipo, tipo_precio).ToList();

                var lista = resultados.Select(item => new
                {
                    item.id,
                    item.nombre,
                    item.descripcion,
                    item.tipo,
                    item.creador,
                    item.lanzamiento,
                    item.Categoria,
                    item.precio,
                    item.tipo_precio
                }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Buscador de Inteligencias Artificiales";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Estudiantes:";
            return View();
        }
    }
}
