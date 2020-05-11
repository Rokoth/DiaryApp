using DB.Context;
using System;
using System.Threading.Tasks;

namespace DB.Repository
{
    public interface IRepository<T> : IRepositoryCommon<T> where T : Entity
    {
        Task<T> Add(T entity, bool withSaving = true);
        Task<T> Delete(Guid id, bool withSaving = true);
        Task<T> Get(Guid id, string include = null);
        Task<T> Update(T entity, bool withSaving = true);
    }

    public interface IRepositoryAll<T> : IRepositoryCommon<T> where T : Entity
    {

    }
}