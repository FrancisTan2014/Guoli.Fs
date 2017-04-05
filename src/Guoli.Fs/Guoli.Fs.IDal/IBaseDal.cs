using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Guoli.Fs.IDal
{
    public interface IBaseDal<T> where T: class,new()
    {
        void Add(T model);

        void Update(T model);

        void Delete(T model);

        IQueryable<T> QueryList(Expression<Func<T, bool>> predicate);

        bool SaveChanges();
    }
}
