using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Guoli.Fs.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // 启用log4net
            var configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
        }
    }
}
