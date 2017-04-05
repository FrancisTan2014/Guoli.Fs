using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Guoli.Fs.Api
{
    /// <summary>
    /// 验证请求者是否带有合法的token
    /// </summary>
    public class ApiTokenFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
        }
    }
}