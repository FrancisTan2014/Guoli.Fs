using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll;
using Guoli.Fs.WebApi.Utils;
using Guoli.Utilities.Helpers;

namespace Guoli.Fs.WebApi.Controllers
{
    public class LoginController : ApiController
    {
        private readonly SystemUserBll _sysUserBll = new SystemUserBll();
        private readonly ViewPersonInfoBll _personInfoBll = new ViewPersonInfoBll();
        private readonly ViewSystemUserBll _viewSystemUserBll = new ViewSystemUserBll();

        [AllowAnonymous]
        public ApiReturns Post(dynamic param)
        {
            var r = HttpContext.Current.Request;
            string account;
            string password;

            try
            {
                account = param.account;
                password = param.password;
            }
            catch (Exception)
            {
                account = r["account"];
                password = r["password"];
            }

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                return ApiReturns.BadRequest();
            }

            var encryptPwd = EncryptHelper.EncryptPassword(password);
            var sysUser = _sysUserBll.QuerySingle(account, encryptPwd);

            if (sysUser != null)
            {
                // 登录成功，返回用户信息
                var user = _viewSystemUserBll.QuerySingle(sysUser.Id);
                var token = LoginStatus.GenerateLoginToken(sysUser.Id, sysUser.Password);

                return ApiReturns.Ok(new { User = user, Token = token });
            }

            return ApiReturns.Unauthorized();
        }
    }
}
