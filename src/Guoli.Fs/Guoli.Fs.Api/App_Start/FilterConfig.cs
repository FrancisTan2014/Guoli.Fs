using System.Web;
using System.Web.Mvc;
using Guoli.Bts.Api;

namespace Guoli.Fs.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ApiTokenFilter());
            filters.Add(new LoginFilter());
            filters.Add(new SuperAdminFilter());
        }
    }
}
