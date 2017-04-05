using System.Web.Mvc;
using Guoli.Bts.Api.Utils;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll;
using Guoli.Utilities.Helpers;

namespace Guoli.Fs.Api.Controllers
{
    public class LoginController : Controller
    {
        private readonly SystemUserBll _sysUserBll = new SystemUserBll();
        private readonly PersonInfoBll _personInfoBll = new PersonInfoBll();

        [AllowAnonymous]
        //[HttpPost]
        public JsonResult Index(string account, string password)
        {
            var encryptPwd = EncryptHelper.EncryptPassword(password);
            var sysUser = _sysUserBll.QuerySingle(account, encryptPwd);

            if (sysUser != null)
            {
                // 登录成功，返回用户信息
                var user = _personInfoBll.QuerySingle(sysUser.PersonInfoId);
                var token = LoginStatus.GenerateLoginToken(sysUser.Id, sysUser.Password);

                var res = ApiReturns.Ok(new {User = user, Token = token});
                return Json(res);
            }

            return Json(ApiReturns.Unauthorized());
        }
    }
}
