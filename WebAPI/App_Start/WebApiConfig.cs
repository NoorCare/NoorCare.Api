using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.EnableCors(new EnableCorsAttribute("http://localhost:4200", headers: "*", methods: "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SupportedMediaTypes
             .Add(new MediaTypeHeaderValue("text/html"));
            config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
