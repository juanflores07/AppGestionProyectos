using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AppGestionProyectos
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name:"CrearTarea",
                url: "Creacion/CrearTarea",
                defaults: new {controller = "Creacion", action = "CrearTarea"}
                );

            routes.MapRoute(
                name: "ListaTareas",
                url: "Creacion/ListaTareas",
                defaults: new { controller = "Creacion", action = "ListaTareas" }
                );

            routes.MapRoute(
                name: "AsignarTarea",
                url: "Creacion/AsignarTarea",
                defaults: new { controller = "Creacion", action = "AsignarTarea" }
                );

            routes.MapRoute(
                name: "EditarTarea",
                url: "Creacion/EditarTarea",
                defaults: new { controller = "Creacion", action = "EditarTarea" }
                );

            routes.MapRoute(
                name: "EliminarTarea",
                url: "Creacion/EliminarTarea",
                defaults: new { controller = "Creacion", action = "EliminarTarea" }
                );

            routes.MapRoute(
                name: "SeguimientoTareas",
                url: "Creacion/SeguimientoTareas",
                defaults: new { controller = "Creacion", action = "SeguimientoTareas" }
                );
            routes.MapRoute(
                name: "DetalleTarea",
                url: "Creacion/DetalleTarea",
                defaults: new { controller = "Creacion", action = "DetalleTarea" }
                );

            routes.MapRoute(
                name: "Inicio",
                url: "Creacion/Inicio",
                defaults: new { controller = "Creacion", action = "Inicio" }
                );

            routes.MapRoute(
                name: "Login",
                url: "Creacion/Login",
                defaults: new { controller = "Creacion", action = "Login" }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Creacion", action = "Login", id = UrlParameter.Optional }
                );



        }
    }
}
