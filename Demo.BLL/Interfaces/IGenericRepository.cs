using Demo.DAL.Entities;
using System.Collections.Generic;

namespace Demo.BLL.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        T GetById(int? id);
        IEnumerable<T> GetAll();
        int Add(T obj);
        int Update(T obj);
        int Delete(T obj);
    }
}
