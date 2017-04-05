using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoli.Fs.Model;

namespace Guoli.Fs.Bll
{
    public partial class OperationLogBll
    {
        public void WriteLog()
        {
            var log = new OperationLog();
            log.OperationTime = DateTime.Now;
            
        }
    }
}
