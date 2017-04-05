using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Guoli.Fs.Api.Models;
using Guoli.Fs.WebApi.Utils;

namespace Guoli.Fs.WebApi
{
    /// <summary>
    /// 对特定的业务逻辑执行超级管理员权限验证
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-20</since>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = true)]
    public class SuperAdminFilterAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            // 超级管理员权限控制
            if (!LoginStatus.IsSuperAdminLogin())
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, ApiReturns.Forbidden());
            }
        }
    }
}