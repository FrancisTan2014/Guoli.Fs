using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Guoli.Fs.Api.Models;
using Guoli.Fs.WebApi.Utils;
using log4net;
using Newtonsoft.Json;

namespace Guoli.Fs.WebApi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LogFilter : ActionFilterAttribute
    {
        readonly ILog _logger = LogManager.GetLogger(nameof(LogFilter));

        public override async void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            var request = actionExecutedContext.Request;
            var sb = new StringBuilder();
            sb.AppendLine()
              .AppendLine($"Request Url: {request.RequestUri}")
              .AppendLine($"Method: {request.Method}")
              .AppendLine(
                    $"Parameters: {JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments)}");

            var res = await actionExecutedContext.Response.Content.ReadAsAsync<ApiReturns>();
            sb.AppendLine($"Response: {JsonConvert.SerializeObject(res)}");

            _logger.Info(sb);
        }
    }
}