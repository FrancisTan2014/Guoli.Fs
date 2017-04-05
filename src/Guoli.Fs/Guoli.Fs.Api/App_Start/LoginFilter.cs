using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Guoli.Bts.Api.Utils;
using Guoli.Fs.Api.Models;
using log4net;

namespace Guoli.Bts.Api
{
    /// <summary>
    /// 身份验证过滤器，对指定控制器或者方法进行登录验证
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-20</since>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LoginFilter : ActionFilterAttribute
    {
        ILog _logger = LogManager.GetLogger(nameof(LoginFilter));

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //_logger.Info("executing LoginFilter");

            // 对使用AllowAnonymous特性标记的Action不执行验证
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Any())
            {
                return;
            }

            if (!LoginStatus.HasLogin(HttpContext.Current.Request))
            {
                //filterContext.Result = new JsonResult { Data = ApiReturns.TokenExpired() };
            }
        }
    }
}