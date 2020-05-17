using System;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
    public interface IDataService<TEntry> where TEntry : Models.Entity
    {
        Task<TEntry> AddItem(TEntry entry);
        Task<TEntry> DeleteItem(Guid id);
        Task<TEntry> GetItem(Guid id);
        Task<TEntry> UpdateItem(TEntry entry);
    }
}
