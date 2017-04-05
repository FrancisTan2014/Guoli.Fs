using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guoli.Fs.Bll
{
    public partial class DepartmentBll
    {
        public bool Delete(int id)
        {
            var model = QuerySingle(id);
            if (model == null)
            {
                return false;
            }

            // 判断是否存在与此部门相关的人员信息
            var personBll = new PersonInfoBll();
            if (personBll.Exists(p => p.IsDeleted == false && p.DepartmentId == id))
            {
                return false;
            }

            // 判断是否存在与此部门相关的文件信息
            var fileBll = new FileDirectoryBll();
            if (fileBll.Exists(f => f.IsDeleted == false && f.DepartmentId == id))
            {
                return false;
            }

            // 尝试删除此部门
            model.IsDeleted = true;
            Dal.Update(model);

            return Dal.SaveChanges();
        }
    }
}
