using System.Linq;
using System.Web.Mvc;
using Guoli.Bts.Api;
using Guoli.Fs.Bll;

namespace Guoli.Fs.Api.Controllers
{
    [SuperAdminFilter]
    public class DepartmentController: Controller
    {
        private readonly DepartmentBll _departmentBll = new DepartmentBll();
        
        public JsonResult GetPage(int page, int size, string app_token)
        {
            var list = _departmentBll.QueryList(d => d.IsDeleted == false)
                .OrderByDescending(d => d.AddTime)
                .Skip((page-1)*size).Take(size);

            return Json(list);
        }
    }
}
