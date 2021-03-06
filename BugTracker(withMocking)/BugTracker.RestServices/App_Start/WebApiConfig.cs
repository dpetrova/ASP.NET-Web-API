﻿using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

namespace BugTracker.RestServices
{
    using Newtonsoft.Json.Serialization;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Disable the XML media formatter (because it cannot serialize annonymous types)
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //to return JSON in camelCase
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = 
                                                new CamelCasePropertyNamesContractResolver();
        }
    }
}
