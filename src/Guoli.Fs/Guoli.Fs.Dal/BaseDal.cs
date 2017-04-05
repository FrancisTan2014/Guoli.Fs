using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Guoli.Fs.Model;

namespace Guoli.Fs.Dal
{
    public class BaseDal<T>
        where T: class,new()
    {
        /// <summary>
        /// 获取当前线程内唯一的DbContext对象
        /// </summary>
        private DbContext CurrentContext
        {
            get
            {
                var name = "DbContext";
                var context = CallContext.LogicalGetData(name) as DbContext;
                if (context == null)
                {
                    context = new BtsEntities();
                    // 创建线程内唯一的DbContext
                    CallContext.LogicalSetData(name, context);
                }

                return context;
            }
        }

        private bool _isModelUnchanged = false;

        protected DbSet<T> Db => CurrentContext.Set<T>();

        public void Add(T model)
        {
            Db.Add(model);
        }

        public void Update(T model)
        {
            Db.AddOrUpdate(model);
            _isModelUnchanged = CurrentContext.Entry(model).State == EntityState.Unchanged;
        }

        public void Delete(T model)
        {
            Db.Remove(model);
        }

        public bool SaveChanges()
        {
            // 若被更新实体字段值与数据库保持一致则直接返回
            if (_isModelUnchanged)
            {
                return true;
            }

            return CurrentContext.SaveChanges() > 0;
        }

        public T QuerySingle(Expression<Func<T, bool>> predicate)
        {
            return Db.SingleOrDefault(predicate);
        }

        public IQueryable<T> QueryList(Expression<Func<T, bool>> predicate)
        {
            return Db.Where(predicate);
        }
        public long GetTotalCount(Expression<Func<T, bool>> predicate)
        {
            return Db.LongCount(predicate);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            
            return Db.Any(predicate);
        }

        public bool ExecuteTransaction(params Func<bool>[] delegates)
        {
            if (delegates == null || !delegates.Any())
            {
                return false;
            }

            using (var scope = new TransactionScope())
            {
                try
                {
                    var success = delegates.Aggregate(true, (current, d) => current && d());
                    if (success)
                    {
                        scope.Complete();
                    }

                    return success;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
