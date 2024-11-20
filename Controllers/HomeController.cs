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

                // Verificar si la búsqueda contiene palabras de 4 o más letras
                bool contienePalabraValida = nombre?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Any(palabra => palabra.Length >= 4) ?? false;

                // Solo guardar en el historial si hay palabras válidas
                if (contienePalabraValida)
                {
                    var historial = new HistorialBusqueda
                    {
                        usuarioId = "12345", // Puedes cambiar esto según tu lógica de usuario
                        nombre = nombre,
                        consulta = nombre,
                        categoria = tipo,
                        tipoPrecio = tipo_precio,
                        fechaBusqueda = DateTime.Now
                    };

                    contexto.HistorialBusqueda.Add(historial);
                    contexto.SaveChanges();
                }

                // Realizar la búsqueda
                var resultados = contexto.buscarPorNombre(nombre, cantidad, null, tipo, tipo_precio).ToList();

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
                    item.tipo_precio,
                    item.Enlace
                }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObtenerRecomendaciones(string usuarioId)
        {
            try
            {
                using (ProyectoBusquedaEntities contexto = new ProyectoBusquedaEntities())
                {
                    // Obtener las búsquedas recientes del usuario
                    var busquedasRecientes = contexto.HistorialBusqueda
                        .Where(h => h.usuarioId == usuarioId)
                        .OrderByDescending(h => h.fechaBusqueda)
                        .Take(5)
                        .ToList();

                    // Obtener categorías y términos de búsqueda recientes
                    var categoriasRecientes = busquedasRecientes
                        .Where(h => !string.IsNullOrEmpty(h.categoria))
                        .Select(h => h.categoria)
                        .Distinct()
                        .ToList();

                    var terminosBusqueda = busquedasRecientes
                        .Where(h => !string.IsNullOrEmpty(h.consulta))
                        .SelectMany(h => h.consulta.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        .Where(term => term.Length >= 4)
                        .Distinct()
                        .ToList();

                    // Construir la consulta base
                    var recomendacionesQuery = contexto.ias.AsQueryable();

                    // Aplicar filtros basados en el historial del usuario
                    if (categoriasRecientes.Any())
                    {
                        recomendacionesQuery = recomendacionesQuery
                            .Where(ia => categoriasRecientes.Contains(ia.Categoria));
                    }

                    // Buscar IAs similares basadas en los términos de búsqueda
                    if (terminosBusqueda.Any())
                    {
                        recomendacionesQuery = recomendacionesQuery
                            .Where(ia => terminosBusqueda.Any(term =>
                                ia.nombre.Contains(term) ||
                                ia.descripcion.Contains(term) ||
                                ia.Categoria.Contains(term) ||
                                ia.creador.Contains(term)));
                    }

                    // Obtener las recomendaciones finales
                    var recomendaciones = recomendacionesQuery
                        .OrderBy(ia => Guid.NewGuid()) // Ordenar aleatoriamente
                        .Take(5)
                        .Select(ia => new
                        {
                            ia.id,
                            ia.nombre,
                            ia.descripcion,
                            ia.tipo,
                            ia.creador,
                            ia.lanzamiento,
                            ia.Categoria,
                            ia.precio,
                            ia.tipo_precio,
                            relevancia = categoriasRecientes.Contains(ia.Categoria) ? "Alta" : "Media"
                        })
                        .ToList();

                    // Si no hay suficientes recomendaciones, agregar algunas populares o aleatorias
                    if (recomendaciones.Count < 5)
                    {
                        var idsExistentes = recomendaciones.Select(r => r.id).ToList();
                        var recomendacionesAdicionales = contexto.ias
                            .Where(ia => !idsExistentes.Contains(ia.id))
                            .OrderBy(ia => Guid.NewGuid())
                            .Take(5 - recomendaciones.Count)
                            .Select(ia => new
                            {
                                ia.id,
                                ia.nombre,
                                ia.descripcion,
                                ia.tipo,
                                ia.creador,
                                ia.lanzamiento,
                                ia.Categoria,
                                ia.precio,
                                ia.tipo_precio,
                                relevancia = "Baja"
                            })
                            .ToList();

                        recomendaciones.AddRange(recomendacionesAdicionales);
                    }

                    return Json(recomendaciones, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObtenerHistorial(string usuarioId)
        {
            try
            {
                using (ProyectoBusquedaEntities contexto = new ProyectoBusquedaEntities())
                {
                    var historial = contexto.HistorialBusqueda
                        .Where(h => h.usuarioId == usuarioId)
                        .OrderByDescending(h => h.fechaBusqueda)
                        .Take(5)
                        .Select(h => new
                        {
                            h.id,
                            h.nombre,
                            h.consulta,
                            h.categoria,
                            h.tipoPrecio,
                            fechaBusqueda = h.fechaBusqueda
                        })
                        .ToList();

                    return Json(historial, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
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