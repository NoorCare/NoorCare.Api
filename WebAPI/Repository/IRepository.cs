using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NoorCare.Repository
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        List<TEntity> GetAll();
        List<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(TKey id);
        TKey Insert(TEntity model);
        bool Update(TEntity model);
        bool Remove(TKey id);
        bool Delete(TKey id);
    }
}
