using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DocsToPictures.NETFrameworkWEB
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            var formatters = GlobalConfiguration.Configuration.Formatters;

            formatters.Remove(formatters.XmlFormatter);
        }
    }
}
