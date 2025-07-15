using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;


namespace Api
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // Log the routes
            //foreach (Route route in routes) { System.Diagnostics.Debug.WriteLine($"Route: {route.Url}"); }

            //routes.MapHttpRoute(
            //name: "API Default",
            //routeTemplate: "api/{controller}/{action}/{id}",
            //defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional });
        }
    }
}
