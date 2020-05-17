using DiaryApp.Models;
using System;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
    public interface IContactDataService : IDataService<Models.Contact>
    {
        Task<ContactInfo> AddContactInfoItem(ContactInfo entry);
        Task<ContactInfo> DeleteContactInfoItem(Guid id);
        Task<ContactInfo> GetContactInfoItem(Guid id);
        Task<Models.FilteredList<Models.Contact>> GetList(int size, int page, string sort, bool? descSort,
            string search, DateTimeOffset? birthDateBegin, DateTimeOffset? birthDateEnd);
        Task<ContactInfo> UpdateContactInfoItem(ContactInfo entry);
    }
}
