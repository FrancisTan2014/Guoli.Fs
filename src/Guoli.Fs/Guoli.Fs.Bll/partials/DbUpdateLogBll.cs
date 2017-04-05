using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoli.Fs.Model;

namespace Guoli.Fs.Bll.partials
{
    public class DbUpdateLogBll: BaseBll<DbUpdateLog>
    {
        public override DbUpdateLog QuerySingle(int id)
        {
            return Dal.QuerySingle(d => d.Id == id);
        }
    }
}
