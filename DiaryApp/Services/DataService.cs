using AutoMapper;
using Contracts;
using DB.Repository;
using DiaryApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
    public class DataService : IDataService
    {
        private readonly IRepositoryAll<DB.Context.Entry> repositoryAllEntry;
        private readonly IMapper mapper;

        public DataService(IRepositoryAll<DB.Context.Entry> repositoryAll, ILogger<DataService> logger, IMapper mapper)
        {
            this.repositoryAllEntry = repositoryAll;
            this.mapper = mapper;
        }

        public async Task<Models.FilteredList<AllEntry>> GetList(
            int size, int page, string sort, bool? descSort, string search,
            DateTimeOffset? beginDate, DateTimeOffset? endDate, EntryType? entryType)
        {
            var filter = new Filter<DB.Context.Entry>()
            {
                Page = page,
                Size = size,
                SortField = sort,
                DescSort = descSort,
                AddFilter = s =>
                    (entryType == null || entryType == EntryType.All || s.EntryType == entryType)
                    && (beginDate == null || s.BeginDate >= beginDate)
                    && (endDate == null || s.BeginDate < endDate)
                    && (string.IsNullOrEmpty(search) || s.Title.Contains(search) || s.Description.Contains(search))
            };

            var entries = await repositoryAllEntry.GetByFilter(filter);

            var result = mapper.Map<Models.FilteredList<AllEntry>>(entries);
            result.BeginNumber = page * size + 1;
            result.EndNumber = page * size + entries.Entries.Count();
            result.IsFirstPage = page == 0;
            result.IsLastPage = page * size + size >= entries.AllCount;
            return result;
        }

        public async Task<GroupedList<AllEntry>> GetDaily(int offset)
        {
            var now = DateTimeOffset.Now.AddDays(offset);
            DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);

            var filter = new Filter<DB.Context.Entry>()
            {
                AddFilter = s => s.BeginDate >= beginDate && s.BeginDate < beginDate.AddDays(1),
                SortField = "BeginDate",
                DescSort = true
            };
            var entries = await repositoryAllEntry.GetByFilter(filter);

            return new GroupedList<AllEntry>()
            {
                Entries = entries.Entries
                    .GroupBy(s => s.BeginDate.ToString("hh:00") + " - " + s.BeginDate.AddHours(1).ToString("hh:00"))
                    .ToDictionary(s => s.Key, s => s.Select(c => mapper.Map<AllEntry>(c))),
                Key = beginDate.ToString("yyyy-MM-dd")
            };
        }

        public async Task<GroupedList<AllEntry>> GetMonthly(int offset)
        {
            var now = DateTimeOffset.Now.AddMonths(offset);
            DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

            var filter = new Filter<DB.Context.Entry>()
            {
                AddFilter = s => s.BeginDate >= beginDate && s.BeginDate < beginDate.AddMonths(1),
                SortField = "BeginDate",
                DescSort = true
            };
            var entries = await repositoryAllEntry.GetByFilter(filter);

            return new GroupedList<AllEntry>()
            {
                Entries = entries.Entries
                    .GroupBy(s => s.BeginDate.ToString("yyyy-MM-dd") + " - " + s.BeginDate.AddDays(1).ToString("yyyy-MM-dd"))
                    .ToDictionary(s => s.Key, s => s.Select(c => mapper.Map<AllEntry>(c))),
                Key = beginDate.ToString("yyyy-MM")
            };
        }

        public async Task<GroupedList<AllEntry>> GetWeekly(int offset)
        {
            var now = DateTimeOffset.Now.AddDays(offset * 7).AddDays(-(int)DateTimeOffset.Now.DayOfWeek);
            DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);

            var filter = new Filter<DB.Context.Entry>()
            {
                AddFilter = s => s.BeginDate >= beginDate && s.BeginDate < beginDate.AddDays(7),
                SortField = "BeginDate",
                DescSort = true
            };
            var entries = await repositoryAllEntry.GetByFilter(filter);

            return new GroupedList<AllEntry>()
            {
                Entries = entries.Entries
                    .GroupBy(s => s.BeginDate.ToString("yyyy-MM-dd") + " - " + s.BeginDate.AddDays(1).ToString("yyyy-MM-dd"))
                    .ToDictionary(s => s.Key, s => s.Select(c => mapper.Map<AllEntry>(c))),
                Key = beginDate.ToString("yyyy-MM-dd") + " - " + beginDate.AddDays(7).ToString("yyyy-MM-dd")
            };
        }
    }

    public class DataService<TEntry, TDbEntry> : IDataService<TEntry>
        where TEntry : Models.Entity
        where TDbEntry : DB.Context.Entity
    {
        protected readonly ILogger<DataService<TEntry, TDbEntry>> logger;
        protected readonly IRepository<TDbEntry> repository;
        protected readonly IMapper mapper;

        public DataService(IRepository<TDbEntry> repository, ILogger<DataService<TEntry, TDbEntry>> logger, IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
        }

        public virtual async Task<TEntry> GetItem(Guid id)
        {
            return await ExecuteSafeAsync(async () => await GetItemInternal(id), nameof(GetItem));
        }

        public virtual async Task<TEntry> AddItem(TEntry entry)
        {
            return await ExecuteSafeAsync(async () => await AddItemInternal(entry), nameof(AddItem));
        }

        public virtual async Task<TEntry> DeleteItem(Guid id)
        {
            return await ExecuteSafeAsync(async () => await DeleteInternal(id), nameof(DeleteItem));
        }

        public virtual async Task<TEntry> UpdateItem(TEntry entry)
        {
            return await ExecuteSafeAsync(async () => await UpdateInternal(entry), nameof(UpdateItem));
        }

        protected virtual async Task<TEntry> UpdateInternal(TEntry entry)
        {
            var result = await repository.Update(mapper.Map<TDbEntry>(entry));
            return mapper.Map<TEntry>(result);
        }

        protected virtual async Task<TEntry> GetItemInternal(Guid id)
        {
            var result = await repository.Get(id);
            return mapper.Map<TEntry>(result);
        }

        protected virtual async Task<TEntry> DeleteInternal(Guid id)
        {
            var result = await repository.Delete(id);
            return mapper.Map<TEntry>(result);
        }

        protected virtual async Task<TEntry> AddItemInternal(TEntry entry)
        {
            var entity = mapper.Map<TDbEntry>(entry);
            entity.Id = Guid.NewGuid();
            entity.VersionDate = DateTimeOffset.Now;
            var result = await repository.Add(entity);
            return mapper.Map<TEntry>(result);
        }

        protected virtual async Task<TEntry> ExecuteSafeAsync(Func<Task<TEntry>> func, string methodName)
        {
            try
            {
                return await func();

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {methodName} : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
