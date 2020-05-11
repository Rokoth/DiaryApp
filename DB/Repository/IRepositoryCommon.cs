using DB.Context;
using System.Threading.Tasks;

namespace DB.Repository
{
    public interface IRepositoryCommon<T> where T : Entity
    {
        Task<FilteredList<T>> GetByFilter(Filter<T> filter, string include = null);
        Task SaveChangesAsync();
    }
}