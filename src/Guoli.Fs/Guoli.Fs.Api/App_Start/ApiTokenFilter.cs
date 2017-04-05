using System.Web.Mvc;
using Guoli.Fs.Api.Models;

namespace Guoli.Fs.Api
{
    /// <summary>
    /// 验证请求者是否带有合法的token
    /// </summary>
    public class ApiTokenFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //filterContext.Result = new JsonResult {Data = ApiReturns.Forbidden()};
        }
    }
}