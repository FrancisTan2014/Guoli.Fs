using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Guoli.Fs.Api.Models;
using Guoli.Fs.Api.Utils;
using Guoli.Fs.Bll;
using Guoli.Fs.Bll.partials;
using Guoli.Fs.Model;
using Guoli.Fs.WebApi.Models;
using Guoli.Fs.WebApi.Utils;
using Guoli.Utilities.Extensions;
using Newtonsoft.Json;

namespace Guoli.Fs.WebApi.Controllers
{
    public class FileController : ApiController
    {
        readonly FileDirectoryBll _dirBll = new FileDirectoryBll();
        readonly ViewDepartFilesBll _fileBll = new ViewDepartFilesBll();
        readonly FileInfoBll _fileInfoBll = new FileInfoBll();
        readonly DepartFilesBll _departFilesBll = new DepartFilesBll();
        readonly DbUpdateLogBll _dbUpdateLogBll = new DbUpdateLogBll();

        readonly object _lockObj = new object();

        public ApiReturns GetDirsAndFiles(int id)
        {
            List<FileDirectory> dirs;
            List<ViewDepartFiles> files;
            if (id == 0)
            {
                // 顶级目录，所有人看到的都一样
                dirs = _dirBll.QueryList(d => d.IsTopestDir && !d.IsDeleted).ToList();
                files = new List<ViewDepartFiles>();
            }
            else
            {
                var loginUser = LoginStatus.GetLoginUser();
                var departId = loginUser.DepartmentId;

                // 子目录，各单位的只能看到自己单位的以及公共的目录
                dirs = _dirBll.QueryList(
                    d => !d.IsDeleted && d.ParentId == id // 定位到当前目录
                    && (d.DepartmentId == departId || d.IsCommon)) // 查询公共的及单位私有的文件夹
                .ToList();

                // 子目录，各单位的只能看到自己单位的以及公共的目录
                files = _fileBll.QueryList(
                    f => !f.IsDeleted && f.FileDirectoryId == id &&
                    (f.IsCommon || f.DepartmentId == departId))
                .ToList();
            }

            return ApiReturns.Ok(new { dirs, files });
        }

        [HttpPost]
        [Route("api/file/getupdated")]
        public ApiReturns GetUpdated()
        {
            var s = HttpContext.Current.Request["ids"];
            var ids = JsonConvert.DeserializeObject<int[]>(s);
            var list = _departFilesBll.QueryList(d => ids.Contains(d.Id));
            return ApiReturns.Ok(list);
        }

        [HttpGet]
        [Route("api/file/check/{start}")]
        public ApiReturns Check(int start)
        {
            var list = _fileInfoBll.QueryList(f => f.Id > start);
            return ApiReturns.Ok(list);
        }

        public ApiReturns Post()
        {
            var request = HttpContext.Current.Request;
            var directoryId = request["dir"].ToInt32();
            if (directoryId <= 0)
            {
                return ApiReturns.BadRequest();
            }

            var directory = _dirBll.QuerySingle(directoryId);
            if (directory == null)
            {
                return ApiReturns.BadRequest();
            }

            var files = request.Files;
            if (files.Count > 0)
            {
                var loginUser = LoginStatus.GetLoginUser();
                var fileIsCommom = LoginStatus.IsSuperAdminLogin() && directory.IsCommon;

                var file = files[0];
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var departFile = new DepartFiles
                {
                    DepartmentId = LoginStatus.IsSuperAdminLogin() ? 0 : loginUser.DepartmentId.Value,
                    FileDirectoryId = directoryId, FileName = fileName, IsCommon = fileIsCommom, SystemUserId = loginUser.Id, LastModifyTime = DateTime.Now
                };

                #region 上传的文件已存在于服务器
                // 对比哈希值，相同文件只保存一份副本
                var hash = file.InputStream.GetMd5();
                var dbFile = _fileInfoBll.QuerySingle(f => f.IsDeleted == false && f.HashCode == hash);
                if (dbFile != null)
                {
                    departFile.FileInfoId = dbFile.Id;
                    
                    var addSuccess = _departFilesBll.ExecuteTranscation(() =>
                    {
                        if (_departFilesBll.Add(departFile).Id > 0)
                        {
                            var log = new DbUpdateLog
                            {
                                OperateType = (int)Operation.Insert,
                                TableName = nameof(DepartFiles),
                                TargetId = departFile.Id,
                                UpdateTime = DateTime.Now
                            };
                            return _dbUpdateLogBll.Add(log).Id > 0;
                        }

                        return false;
                    });
                    var res = _fileBll.QuerySingle(departFile.Id);

                    if (addSuccess)
                    {
                        return ApiReturns.Created(res);
                    }

                    return ApiReturns.BadRequest();
                }
                #endregion

                #region 上传的文件未存在于服务器
                // 路径
                var d = DateTime.Now;
                var ext = Path.GetExtension(file.FileName);
                var dir = $"/docs/{d.Year}-{d.Month}-{d.Day}/";
                var name = $"{Guid.NewGuid()}{ext}";
                var path = $"{WebConfig.FileUploadDir}{dir}";
                if (!Directory.Exists(path))
                {
                    lock (_lockObj)
                    {
                        Directory.CreateDirectory(path);
                    }
                }

                // 保存文件到磁盘
                file.SaveAs($"{path}{name}");

                // 保存到数据库
                var serverFileName = $"{dir}{name}";
                dbFile = new Guoli.Fs.Model.FileInfo
                {
                    Extension = ext,
                    HashCode = hash,
                    Path = serverFileName,
                    Size = file.InputStream.Length,
                    UploadTime = d
                };

                var success = _fileInfoBll.ExecuteTranscation(() =>
                {
                    var s = _fileInfoBll.Add(dbFile).Id > 0;
                    if (s)
                    {
                        departFile.FileInfoId = dbFile.Id;

                        if (_departFilesBll.Add(departFile).Id > 0)
                        {
                            var log = new DbUpdateLog
                            {
                                OperateType = (int)Operation.Insert,
                                TableName = nameof(DepartFiles),
                                TargetId = departFile.Id,
                                UpdateTime = DateTime.Now
                            };
                            return _dbUpdateLogBll.Add(log).Id > 0;
                        }

                        return false;
                    }

                    return false;
                });

                if (success)
                {
                    var data = _fileBll.QuerySingle(departFile.Id);
                    return ApiReturns.Created(data);
                }

                return ApiReturns.BadRequest();
                #endregion
            }

            return ApiReturns.BadRequest();
        }

        [HttpPut]
        [Route("api/file/rename/{id}/{newName}")]
        public ApiReturns Put(int id, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return ApiReturns.BadRequest();
            }

            var file = _departFilesBll.QuerySingle(id);
            if (file == null)
            {
                return ApiReturns.BadRequest();
            }
            
            if (file.IsCommon && !LoginStatus.IsSuperAdminLogin())
            {
                return ApiReturns.Forbidden();
            }

            file.FileName = newName;

            var success = UpdateFile(file, Operation.Update);
            if (success)
            {
                return ApiReturns.Created();
            }

            return ApiReturns.Failed();
        }

        public ApiReturns Delete(int id)
        {
            var file = _departFilesBll.QuerySingle(id);
            if (file == null)
            {
                return ApiReturns.NotFound();
            }

            if (file.IsCommon && !LoginStatus.IsSuperAdminLogin())
            {
                return ApiReturns.Forbidden();
            }

            file.IsDeleted = true;
            var success = UpdateFile(file, Operation.Delete);
            if (success)
            {
                return ApiReturns.NoContent();
            }

            return ApiReturns.Failed();
        }

        /// <summary>
        /// 更新文件信息，并添加数据库更新记录
        /// </summary>
        /// <param name="file">待更新的文件信息</param>
        /// <param name="operation">表示数据库操作类型（增删改）的枚举值</param>
        /// <returns>更新成功则返回<c>true</c>，否则返回<c>false</c></returns>
        private bool UpdateFile(DepartFiles file, Operation operation)
        {
            return _departFilesBll.ExecuteTranscation(() =>
            {
                var s = _departFilesBll.Update(file);
                if (s)
                {
                    var d = new DbUpdateLog
                    {
                        OperateType = (int)operation,
                        TableName = nameof(DepartFiles),
                        TargetId = file.Id,
                        UpdateTime = DateTime.Now
                    };

                    return _dbUpdateLogBll.Add(d).Id > 0;
                }

                return false;
            });
        }
    }
}
