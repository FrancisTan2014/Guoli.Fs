using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoli.Fs.Model;

namespace Guoli.Fs.Bll.partials
{
    public partial class ViewDepartFilesBll: BaseBll<ViewDepartFiles>
    {
        public override ViewDepartFiles QuerySingle(int id)
        {
            return Dal.QuerySingle(f => f.Id == id && f.IsDeleted == false);
        }
    }
}
