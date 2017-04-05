using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Guoli.Fs.Dal;
using Guoli.Utilities.Helpers;

namespace Guoli.Fs.Bll
{
    public abstract class BaseBll<T> where T : class, new()
    {
        protected BaseDal<T> Dal { get; }

        protected BaseBll()
        {
            var assembly = "Guoli.Fs.Dal";
            var fullName = $"Guoli.Fs.Dal.{typeof(T).Name}Dal";
            Dal = ReflectorHelper.GetInstance(assembly, fullName) as BaseDal<T>;
        }

        public T Add(T model)
        {
            Dal.Add(model);
            Dal.SaveChanges();

            return model;
        }

        public bool Update(T model)
        {
            Dal.Update(model);

            return Dal.SaveChanges();
        }

        public bool Delete(T model)
        {
            Dal.Delete(model);

            return Dal.SaveChanges();
        }

        public abstract T QuerySingle(int id);

        public long GetTotalCount(Expression<Func<T, bool>> predicate)
        {
            return Dal.GetTotalCount(predicate);
        }

        public T QuerySingle(Expression<Func<T, bool>> predicate)
        {
            return Dal.QuerySingle(predicate);
        }

        public IQueryable<T> QueryList(Expression<Func<T, bool>> predicate)
        {
            return Dal.QueryList(predicate);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return Dal.Exists(predicate);
        }

        public bool ExecuteTranscation(params Func<bool>[] delegates)
        {
            return Dal.ExecuteTransaction(delegates);
        }
    }
}
