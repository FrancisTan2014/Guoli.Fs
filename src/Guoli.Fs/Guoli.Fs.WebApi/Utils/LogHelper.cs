using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Guoli.Fs.Bll;
using Guoli.Fs.Model;
using Guoli.Fs.WebApi.Models;
using log4net;

namespace Guoli.Fs.WebApi.Utils
{
    /// <summary>
    /// 提供记录管理员操作日志的静态方法
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2017-03-27</since>
    public static class LogHelper
    {
        /// <summary>
        /// 向数据库中写入一条管理员操作日志记录
        /// </summary>
        /// <param name="opType">表示对数据操作的类型的枚举值</param>
        /// <param name="desc">本次操作的描述信息</param>
        /// <param name="tableName">被操作的表格名称</param>
        /// <param name="id">被操作的数据的主键值</param>
        public static void WriteOperateLog(Operation opType, string desc, string tableName, int id)
        {
            var loginUser = LoginStatus.GetLoginUser();

            var log = new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationDesc = desc,
                OperationType = (int)opType,
                SystemUserId = loginUser.Id,
                TargetTableName = tableName,
                TargetId = id
            };

            var bll = new OperationLogBll();
            bll.Add(log);
        }

        /// <summary>
        /// 向数据库中写入一条系统日志记录
        /// </summary>
        /// <param name="message">要记录的日志信息</param>
        /// <param name="exception">异常信息对象</param>
        /// <param name="prefix">日志前缀，为null则默认将当前登录人的信息作为前缀</param>
        /// <param name="fileName">文件名称</param>
        public static void WriteSystemLog(string message, Exception exception = null, string prefix = null)
        {
            var logger = LogManager.GetLogger(nameof(LogHelper));
            var loginUser = LoginStatus.GetLoginUser();

            if (string.IsNullOrEmpty(prefix))
            {
                prefix = $"登录用户为[Id:{loginUser.Id}][Name:{loginUser.Name}]";
            }

            logger.Warn($"{prefix} {message}", exception);
        }
    }
}