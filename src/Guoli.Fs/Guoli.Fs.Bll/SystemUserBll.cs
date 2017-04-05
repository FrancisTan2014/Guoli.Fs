using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoli.Fs.Model;

namespace Guoli.Fs.Bll
{
    public partial class SystemUserBll
    {
        public SystemUser QuerySingle(string account, string password)
        {
            return Dal.QuerySingle(s => s.Username == account && s.Password == password);
        }
    }
}
