using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoli.Fs.Model;

namespace Guoli.Fs.Bll
{
    public partial class PersonInfoBll
    {
        /// <summary>
        /// 将相关联的<see cref="PersonInfo"></see>对象和<see cref="SystemUser"></see>对象以事务的方式插入数据库中
        /// </summary>
        /// <param name="person"><see cref="PersonInfo"></see>的实例</param>
        /// <param name="user"><see cref="SystemUser"></see>的实例</param>
        /// <returns>事务执行成功则返回<c>true</c>，否则返回<c>false</c></returns>
        public bool AddPeronAndSystemUser(PersonInfo person, SystemUser user)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Dal.ExecuteTransaction(() =>
            {
                Add(person);
                if (person.Id > 0)
                {
                    var systemUserBll = new SystemUserBll();
                    user.PersonInfoId = person.Id;
                    systemUserBll.Add(user);
                    if (user.Id > 0)
                    {
                        return true;
                    }
                }

                return false;
            });
        }
    }
}
