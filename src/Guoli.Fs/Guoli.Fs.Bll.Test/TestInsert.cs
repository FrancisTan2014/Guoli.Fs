using System;
using Guoli.Fs.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guoli.Fs.Bll.Test
{
    [TestClass]
    public class TestInsert
    {
        [TestMethod]
        public void TestAddOperateLog()
        {
            var bll = new OperationLogBll();
            var log = new OperationLog
            {
                OperationDesc = "测试",
                OperationTime = DateTime.Now,
                OperationType = 1,
                SystemUserId = 1,
                TargetTableName = "测试",
                TargetId = 1
            };

            bll.Add(log);
            Assert.AreEqual(true, log.Id > 0);
        }
    }
}
