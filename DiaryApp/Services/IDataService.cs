using Contracts;
using DiaryApp.Models;
using System;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
    public interface IDataService
    {
        Task<GroupedList<AllEntry>> GetDaily(int offset);

        Task<Models.FilteredList<AllEntry>> GetList(int size, int page, string sort, bool? descSort, string search, DateTimeOffset? beginDate, DateTimeOffset? endDate, EntryType? entryType);
        Task<GroupedList<AllEntry>> GetMonthly(int offset);
        Task<GroupedList<AllEntry>> GetWeekly(int offset);
    }
}
