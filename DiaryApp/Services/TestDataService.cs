using Contracts;
using DB.Repository;
using DiaryApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
    public class TestDataService : IDataService
    {
        public TestDataService(IRepositoryAll<DB.Context.Entry> repositoryAll, ILogger<DataService> logger)
        {

        }


        public async Task<Models.FilteredList<AllEntry>> GetList(int size, int page, string sort, bool? descSort, string search,
            DateTimeOffset? beginDate, DateTimeOffset? endDate, EntryType? entryType)
        {
            return await Task.Run(() =>
            {
                var entriesTemp = Enumerable.Range(page * size + 1, size).Select(s => new AllEntry()
                {
                    BeginDate = DateTimeOffset.Now + TimeSpan.FromHours(s),
                    Description = "Description " + s,
                    Id = Guid.NewGuid(),
                    EntryType = EntryType.Deal
                });

                Models.FilteredList<AllEntry> entries = new Models.FilteredList<AllEntry>()
                {
                    Entries = entriesTemp,
                    AllCount = 1000,
                    BeginNumber = size * page + 1,
                    EndNumber = size * page + entriesTemp.Count()
                };

                return entries;
            });
        }

        public async Task<GroupedList<AllEntry>> GetDaily(int offset)
        {
            return await Task.Run(() =>
            {
                var now = DateTimeOffset.Now.AddDays(offset);
                DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);

                var entries = Enumerable.Range(0, 32).Select(s => new AllEntry()
                {
                    BeginDate = beginDate + TimeSpan.FromHours(1 + s / 2),
                    Description = "Description " + s,
                    Id = Guid.NewGuid(),
                    EntryType = EntryType.Deal
                });

                return new GroupedList<AllEntry>()
                {
                    Entries = entries.GroupBy(s => s.BeginDate.Hour.ToString())
                        .ToDictionary(s => s.Key, s => s.Select(c => c))
                };
            });
        }

        public async Task<GroupedList<AllEntry>> GetMonthly(int offset)
        {
            return await Task.Run(() =>
            {
                var now = DateTimeOffset.Now.AddMonths(offset);
                DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

                var entries = Enumerable.Range(0, 210).Select(s => new AllEntry()
                {
                    BeginDate = beginDate + TimeSpan.FromHours(3 * s),
                    Description = "Description " + s,
                    Id = Guid.NewGuid(),
                    EntryType = EntryType.Deal
                });

                return new GroupedList<AllEntry>()
                {
                    Entries = entries.GroupBy(s => s.BeginDate.Day.ToString())
                        .ToDictionary(s => s.Key, s => s.Select(c => c))
                };
            });
        }

        public async Task<GroupedList<AllEntry>> GetWeekly(int offset)
        {
            return await Task.Run(() =>
            {
                var now = DateTimeOffset.Now.AddDays(offset * 7).AddDays(-(int)DateTimeOffset.Now.DayOfWeek);
                DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, now.Day - (int)now.DayOfWeek, 0, 0, 0, now.Offset);

                var entries = Enumerable.Range(0, 50).Select(s => new AllEntry()
                {
                    BeginDate = beginDate + TimeSpan.FromHours(3 * s),
                    Description = "Description " + s,
                    Id = Guid.NewGuid(),
                    EntryType = EntryType.Deal
                });

                return new GroupedList<AllEntry>()
                {
                    Entries = entries.GroupBy(s => s.BeginDate.Day.ToString())
                        .ToDictionary(s => s.Key, s => s.Select(c => c))
                };
            });
        }
    }
}
