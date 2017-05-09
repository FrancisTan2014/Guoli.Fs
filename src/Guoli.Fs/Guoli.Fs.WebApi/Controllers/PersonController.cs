using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll;
using Guoli.Fs.Model;
using Guoli.Fs.WebApi.Models;
using System;
using System.Linq;
using System.Web.Http;
using Guoli.Fs.WebApi.Utils;
using Guoli.Utilities.Helpers;

namespace Guoli.Fs.WebApi.Controllers
{
    public class PersonController : ApiController
    {
        private readonly PersonInfoBll _personBll = new PersonInfoBll();
        private readonly ViewPersonInfoBll _viewPersonBll = new ViewPersonInfoBll();
        private readonly SystemUserBll _systemUserBll = new SystemUserBll();

        [HttpPost]
        [Route("api/person/getpage")]
        public ApiReturns GetPage(GetPageParams param)
        {
            if (!ModelState.IsValid)
            {
                return ApiReturns.BadRequest();
            }

            var name = param.conditions.Value<string>("name");
            var departId = param.conditions.Value<int>("departId");
            var userType = param.conditions.Value<int>("userType");

            var list = _viewPersonBll.QueryList(p => p.IsDeleted == false);

            #region 条件过滤
            // 条件查询
            if (LoginStatus.IsSuperAdminLogin())
            {
                if (departId > 0)
                {
                    list = list.Where(p => p.DepartmentId == departId);
                }
                if (userType >= 0)
                {
                    list = list.Where(p => p.UserType == userType);
                }
            }
            else
            {
                // 一般管理员登录，仅查询其所属单位的人员信息
                var user = LoginStatus.GetLoginUser();
                list = list.Where(p => p.DepartmentId == user.DepartmentId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(p => p.Name.Contains(name) || p.WorkNo.Contains(name));
            } 
            #endregion

            // 分页
            list = list.OrderByDescending(p => p.Id)
                       .Skip((param.page - 1) * param.size)
                       .Take(param.size);
            var total = list.Count();

            return ApiReturns.Ok(new { total, list });
        }

        public ApiReturns Post(ViewPersonInfo viewPerson)
        {
            var loginUser = LoginStatus.GetLoginUser();
            // 权限验证，非超级管理员不允许添加超级管理员账户，不允许添加不属于其所属单位的人员信息
            if (!LoginStatus.IsSuperAdminLogin() && (viewPerson.UserType == 0 || viewPerson.DepartmentId != loginUser.DepartmentId))
            {
                return ApiReturns.Forbidden();
            }

            viewPerson.AddTime = DateTime.Now;

            var person = new PersonInfo
            {
                AddTime = DateTime.Now,
                Name = viewPerson.Name,
                DepartmentId = viewPerson.DepartmentId,
                Gender = viewPerson.Gender,
                HeadPortraitPath = viewPerson.HeadPortraitPath ?? string.Empty,
                WorkNo = viewPerson.WorkNo
            };
            var user = new SystemUser
            {
                Username = viewPerson.Username,
                Password = EncryptHelper.EncryptPassword(viewPerson.Password),
                UserType = viewPerson.UserType ?? 0
            };

            var success = _personBll.AddPeronAndSystemUser(person, user);
            return success ? ApiReturns.Created() : ApiReturns.BadRequest();
        }

        public ApiReturns Put(int id, ViewPersonInfo viewPerson)
        {
            var loginUser = LoginStatus.GetLoginUser();
            // 权限验证，非超级管理员不允许将账户修改为超级管理员账户，不允许添加不属于其所属单位的人员信息
            if (!LoginStatus.IsSuperAdminLogin() && (viewPerson.UserType == 0 || viewPerson.DepartmentId != loginUser.DepartmentId))
            {
                return ApiReturns.Forbidden();
            }

            var person = _personBll.QuerySingle(id);
            var user = _systemUserBll.QuerySingle(s => s.PersonInfoId == id);
            if (person == null || user == null)
            {
                return ApiReturns.BadRequest();
            }

            person.Name = viewPerson.Name;
            person.DepartmentId = viewPerson.DepartmentId;
            person.Gender = viewPerson.Gender;
            person.HeadPortraitPath = viewPerson.HeadPortraitPath;
            person.WorkNo = viewPerson.WorkNo;

            user.Username = viewPerson.Username;
            user.UserType = viewPerson.UserType.Value;

            var success = _personBll.ExecuteTranscation(() => _personBll.Update(person), () => _systemUserBll.Update(user));
            return success ? ApiReturns.Created() : ApiReturns.BadRequest();
        }

        [HttpPost]
        [Route("api/person/username_exists/{personId}/{username}")]
        public ApiReturns Exists(int personId, string username)
        {
            var exists = !string.IsNullOrEmpty(username) && _systemUserBll.Exists(s => s.IsDeleted == false && s.PersonInfoId != personId && s.Username == username);
            return ApiReturns.Ok(new { exists });
        }

        public ApiReturns Delete(int id)
        {
            var person = _personBll.QuerySingle(id);
            var user = _systemUserBll.QuerySingle(s => s.PersonInfoId == id);

            var loginUser = LoginStatus.GetLoginUser();
            // 权限验证，非超级管理员不允许删除非本单位的账户
            if (!LoginStatus.IsSuperAdminLogin() && person.DepartmentId != loginUser.DepartmentId)
            {
                return ApiReturns.Forbidden();
            }

            person.IsDeleted = true;
            user.IsDeleted = true;

            var success = _personBll.ExecuteTranscation(() => _personBll.Update(person) && _systemUserBll.Update(user));
            return success ? ApiReturns.NoContent() : ApiReturns.BadRequest();
        }
    }
}
