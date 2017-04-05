using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll;
using Guoli.Fs.Model;
using Guoli.Fs.WebApi.Models;
using Guoli.Fs.WebApi.Utils;
using log4net;

namespace Guoli.Fs.WebApi.Controllers
{
    public class DepartmentController: ApiController
    {
        private readonly DepartmentBll _departmentBll = new DepartmentBll();

        public ApiReturns Get()
        {
            var list = _departmentBll.QueryList(d => d.IsDeleted == false);

            return ApiReturns.Ok(list);
        }

        [Route("api/department/getpage", Name = "GetPage", Order = 0)]
        [HttpPost]
        public ApiReturns Get(GetPageParams param)
        {
            var page = param.page;
            var size = param.size;
            var name = param.conditions.Value<string>("name");

            Expression<Func<Department, bool>> predicate;
            if (string.IsNullOrEmpty(name))
            {
                predicate = d => d.IsDeleted == false;
            }
            else
            {
                predicate = d => d.IsDeleted == false && d.Name.Contains(name);
            }

            var totalCount = _departmentBll.GetTotalCount(predicate);
            var list = _departmentBll.QueryList(predicate)
                .OrderByDescending(d => d.AddTime)
                .Skip((page-1)*size)
                .Take(size);

            return ApiReturns.Ok(new { total = totalCount, list});
        }

        [Route("api/department/exists/{name}")]
        [HttpPost]
        [SuperAdminFilter]
        public ApiReturns Exists(string name)
        {
            var exists = _departmentBll.Exists(d => d.IsDeleted == false && d.Name == name);

            return ApiReturns.Ok(new { exists });
        }
        
        public ApiReturns Post(Department model)
        {
            model.AddTime = DateTime.Now;
            model = _departmentBll.Add(model);
            
            return ApiReturns.Created(model);
        }

        [SuperAdminFilter]
        public ApiReturns Put(int id, Department model)
        {
            if (string.IsNullOrEmpty(model?.Name))
            {
                return ApiReturns.BadRequest();
            }

            var depart = _departmentBll.QuerySingle(id);
            depart.Name = model.Name;

            var success = _departmentBll.Update(depart);
            return success ? ApiReturns.Created(depart) : ApiReturns.BadRequest();
        }

        [SuperAdminFilter]
        public ApiReturns Delete(int id)
        {
            var success = _departmentBll.Delete(id);
            return success ? ApiReturns.NoContent() : ApiReturns.BadRequest();
        }

        
    }
}
