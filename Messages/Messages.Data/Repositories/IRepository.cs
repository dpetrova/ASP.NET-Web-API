using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Data.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> All();
        T Find(object id);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        T Remove(object id);
        void SaveChanges();
    }
}
