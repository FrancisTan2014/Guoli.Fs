using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Guoli.Fs.Api.Models;
using Guoli.Fs.WebApi.Utils;
using log4net;

namespace Guoli.Fs.WebApi
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

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            //_logger.Info("executing LoginFilter");

            // 对使用AllowAnonymous特性标记的Action不执行验证
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            // 从url中获取app_token
            var token = string.Empty;
            var query = actionContext.Request.RequestUri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var match = Regex.Match(query, $@"\?.*?{Literals.AppTokenName}=([^&]+)&*");
                token = match.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(token))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, ApiReturns.Forbidden());
            }

            CallContext.SetData(Literals.AppTokenName, token);
            if (!LoginStatus.HasLogin(token))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK,
                    ApiReturns.TokenExpired());
            }
        }
    }
}