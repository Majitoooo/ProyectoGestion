﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_Gestión.ADO
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ProyectoBusquedaEntities : DbContext
    {
        public ProyectoBusquedaEntities()
            : base("name=ProyectoBusquedaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ias> ias { get; set; }
        public virtual DbSet<HistorialBusqueda> HistorialBusqueda { get; set; }
    
        public virtual ObjectResult<buscarPorNombre_Result> buscarPorNombre(string consulta, Nullable<int> cantidad, string categoria, string tipo, string tipo_precio)
        {
            var consultaParameter = consulta != null ?
                new ObjectParameter("consulta", consulta) :
                new ObjectParameter("consulta", typeof(string));
    
            var cantidadParameter = cantidad.HasValue ?
                new ObjectParameter("cantidad", cantidad) :
                new ObjectParameter("cantidad", typeof(int));
    
            var categoriaParameter = categoria != null ?
                new ObjectParameter("Categoria", categoria) :
                new ObjectParameter("Categoria", typeof(string));
    
            var tipoParameter = tipo != null ?
                new ObjectParameter("tipo", tipo) :
                new ObjectParameter("tipo", typeof(string));
    
            var tipo_precioParameter = tipo_precio != null ?
                new ObjectParameter("tipo_precio", tipo_precio) :
                new ObjectParameter("tipo_precio", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<buscarPorNombre_Result>("buscarPorNombre", consultaParameter, cantidadParameter, categoriaParameter, tipoParameter, tipo_precioParameter);
        }
    }
}
