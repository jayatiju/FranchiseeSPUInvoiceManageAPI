using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Enable CORS
            //  var cors = new EnableCorsAttribute("*", "Content-Type, X-Your-Extra-Header-Key", "GET,POST,PUT,DELETE,OPTIONS");
            //  var cors = new EnableCorsAttribute("https://frspuinv.ifbsupport.com:8080", "Content-Type, X-Your-Extra-Header-Key", "GET,POST,PUT,DELETE,OPTIONS");
            //  var cors = new EnableCorsAttribute("https://192.168.52.187:8080", "Content-Type, X-Your-Extra-Header-Key", "GET,POST,PUT,DELETE,OPTIONS");

            var cors = new EnableCorsAttribute("*", "Content-Type, X-Your-Extra-Header-Key", "GET,POST,PUT,DELETE,OPTIONS");
          config.EnableCors(cors);
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            
        }
    }
}
