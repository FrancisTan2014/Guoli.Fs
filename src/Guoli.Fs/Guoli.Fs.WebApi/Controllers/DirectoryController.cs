using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Bll;
using Guoli.Fs.Bll.partials;
using Guoli.Fs.Model;
using Guoli.Fs.WebApi.Models;
using Guoli.Fs.WebApi.Utils;
using Newtonsoft.Json;

namespace Guoli.Fs.WebApi.Controllers
{
    public class DirectoryController : ApiController
    {
        readonly FileDirectoryBll _dirBll = new FileDirectoryBll();
        readonly DbUpdateLogBll _logBll = new DbUpdateLogBll();

        public ApiReturns Get(int id)
        {
            var list = _dirBll.QueryList(d => d.ParentId == id && d.IsDeleted == false);
            return ApiReturns.Ok(list);
        }
        
        [HttpPost]
        [Route("api/directory/getupdated")]
        public ApiReturns GetUpdated()
        {
            var s = HttpContext.Current.Request["ids"];
            var ids = JsonConvert.DeserializeObject<int[]>(s);
            var list = _dirBll.QueryList(d => ids.Contains(d.Id));
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
            var success = _dirBll.ExecuteTranscation(() =>
            {
                var s = _dirBll.Add(model).Id > 0;
                if (s)
                {
                    var d = new DbUpdateLog(nameof(FileDirectory), model.Id, (int)Operation.Insert);
                    return _logBll.Add(d).Id > 0;
                }
                return false;
            });
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
            var success = UpdateDir(dir, Operation.Update);
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

            var success = UpdateDir(dir, Operation.Delete);
            if (success)
            {
                return ApiReturns.NoContent();
            }

            return ApiReturns.Failed();
        }

        /// <summary>
        /// 更新目录信息，并记录数据库更新记录
        /// </summary>
        /// <param name="dir">待更新目录信息</param>
        /// <param name="operation">表示数据库操作类型（增删改）的枚举值</param>
        /// <returns>更新成功则返回<c>true</c>，否则返回<c>false</c></returns>
        private bool UpdateDir(FileDirectory dir, Operation operation)
        {
            return _dirBll.ExecuteTranscation(() =>
            {
                var success = _dirBll.Update(dir);
                if (success)
                {
                    var d = new DbUpdateLog(nameof(FileDirectory), dir.Id, (int)operation);
                    return _logBll.Add(d).Id > 0;
                }
                return false;
            });
        }
    }
}
