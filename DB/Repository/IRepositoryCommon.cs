using DB.Context;
using System.Threading.Tasks;

namespace DB.Repository
{
    public interface IRepositoryCommon<T> where T : Entry
    {
        Task<FilteredList<T>> GetByFilter(Filter<T> filter);
    }
}