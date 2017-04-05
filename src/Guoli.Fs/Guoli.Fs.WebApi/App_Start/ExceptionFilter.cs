using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Guoli.Fs.Api.Models;

namespace Guoli.Fs.WebApi
{
    public class ExceptionFilter: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);

            var logger = log4net.LogManager.GetLogger(nameof(ExceptionFilter));
            logger.Error(actionExecutedContext.Exception?.Message, actionExecutedContext.Exception);
            
            actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, ApiReturns.BadRequest());
        }
    }
}