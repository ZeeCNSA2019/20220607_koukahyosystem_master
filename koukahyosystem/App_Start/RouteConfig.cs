using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace koukahyosystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // IMPORTANT: put this *BEFORE* any convention-based routing rule
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "HomeAbout",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );

            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "HomeContact",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Contact", id = UrlParameter.Optional }
            );

            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "HomeIndex",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //    name: "HomeMaster",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            //);
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "HomeMaster",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "HomeLeader",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Leader", id = UrlParameter.Optional }
            );

           
        
        }
    }
}
