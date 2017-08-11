using DocsToPictures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DocsToPictures
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterService();
        }


        private void RegisterService()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = client.GetStringAsync($"http://ipresolver.azurewebsites.net/ip/SetCCFEndPoint?interfaceName={typeof(IDocumentProccessor).FullName}&url=doctopic/CCF").Result;
                    if (result == "OK")
                    {
                        Console.WriteLine($"Registrate as {typeof(IDocumentProccessor).FullName} success");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }
            catch
            {
                Console.WriteLine("http request error!!! :(");
            }
        }

    }
}
