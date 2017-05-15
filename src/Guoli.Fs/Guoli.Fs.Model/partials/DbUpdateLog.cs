using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guoli.Fs.Model
{
    public partial class DbUpdateLog
    {
        public DbUpdateLog()
        {
            
        }

        public DbUpdateLog(string tableName, int targetId, int operateType)
        {
            TableName = tableName;
            TargetId = targetId;
            OperateType = operateType;
            UpdateTime = DateTime.Now;
        }
    }
}
