using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll;
using Guoli.Fs.Model;
using Guoli.Fs.WebApi.Utils;

namespace Guoli.Fs.WebApi.Controllers
{
    public class DirectoryController : ApiController
    {
        readonly FileDirectoryBll _dirBll = new FileDirectoryBll();

        public ApiReturns Get(int id)
        {
            var list = _dirBll.QueryList(d => d.ParentId == id && d.IsDeleted == false);
            return ApiReturns.Ok(list);
        }

        private bool DirNameExists(FileDirectory dir)
        {
            var exists = false;

            var list = _dirBll.QueryList(d => 
                !d.IsDeleted && 
                d.ParentId == dir.ParentId && 
                d.Id != dir.Id && 
                d.DirName == dir.DirName
            );

            if (list.Any())
            {
                exists = (dir.IsCommon && list.Any(m => m.IsCommon && m.DirName == dir.DirName)) // 验证公共文件夹是否重名
                         ||
                         (!dir.IsCommon && list.Any(m => !m.IsCommon && m.DirName == dir.DirName && m.DepartmentId == dir.DepartmentId)); // 验证各单位文件夹是否重名
            }

            return exists;
        }

        public ApiReturns Post(FileDirectory model)
        {
            // 只有超级管理员才能添加顶级目录
            if (model.ParentId == 0 && !LoginStatus.IsSuperAdminLogin())
            {
                return ApiReturns.Forbidden();
            }
            
            if (DirNameExists(model))
            {
                return ApiReturns.Exists();
            }

            model.CreateTime = DateTime.Now;
            model.LastModifyTime = DateTime.Now;
            model.CreatorId = LoginStatus.GetLoginUser().Id;

            // 插入数据库
            var success = _dirBll.Add(model).Id > 0;
            if (success)
            {
                return ApiReturns.Created(model);
            }

            return ApiReturns.BadRequest();
        }

        [HttpPost]
        [Route("api/directory/{id}/{newName}")]
        public ApiReturns Rename(int id, string newName)
        {
            var dir = _dirBll.QuerySingle(id);
            if (dir == null)
            {
                return ApiReturns.BadRequest();
            }
            if (string.IsNullOrEmpty(newName) || dir.DirName == newName)
            {
                return ApiReturns.BadRequest();
            }

            dir.DirName = newName;
            dir.LastModifyTime = DateTime.Now;
            // 检查是否重名
            if (DirNameExists(dir))
            {
                return ApiReturns.Exists();
            }

            // 更新
            var success = _dirBll.Update(dir);
            if (success)
            {
                return ApiReturns.Created();
            }

            return ApiReturns.BadRequest();
        }

        public ApiReturns Delete(int id)
        {
            var dir = _dirBll.QuerySingle(id);

            // 顶级目录删除权限控制
            if (dir.ParentId == 0 && !LoginStatus.IsSuperAdminLogin())
            {
                return ApiReturns.Forbidden();
            }

            dir.IsDeleted = true;
            var success = _dirBll.Update(dir);
            if (success)
            {
                return ApiReturns.NoContent();
            }

            return ApiReturns.Failed();
        }
    }
}
