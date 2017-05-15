using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll.partials;

namespace Guoli.Fs.WebApi.Controllers
{
    public class UpdateController : ApiController
    {
        readonly DbUpdateLogBll _logBll = new DbUpdateLogBll();

        [HttpGet]
        [Route("api/update/check/{start}")]
        public ApiReturns Check(int start)
        {
            var list = _logBll.QueryList(d => d.Id > start);
            return ApiReturns.Ok(list);
        }
    }
}
