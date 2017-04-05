using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Guoli.Fs.Api;

namespace Guoli.Fs.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // 过滤器
            config.Filters.Add(new LoginFilter());
            config.Filters.Add(new LogFilter());
            config.Filters.Add(new ModelStateFilter());
            config.Filters.Add(new ExceptionFilter());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
