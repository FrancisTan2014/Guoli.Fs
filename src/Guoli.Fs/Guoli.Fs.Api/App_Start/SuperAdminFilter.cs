using System;
using System.Web.Mvc;
using log4net;

namespace Guoli.Bts.Api
{
    /// <summary>
    /// 对特定的业务逻辑执行超级管理员权限验证
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-20</since>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = true)]
    public class SuperAdminFilter: ActionFilterAttribute
    {
        ILog _logger = log4net.LogManager.GetLogger(nameof(SuperAdminFilter));

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //_logger.Info("executing SuperAdminFilter");
            
            base.OnActionExecuting(filterContext);
        }
    }
}