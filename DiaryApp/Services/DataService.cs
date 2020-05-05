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
        private readonly IRepositoryAll repositoryAll;
        private readonly IMapper mapper;

        public DataService(IRepositoryAll repositoryAll, ILogger<DataService> logger, IMapper mapper)
        {
            this.repositoryAll = repositoryAll;
            this.mapper = mapper;
        }

        public async Task<Models.FilteredList<AllEntry>> GetList(
            int size, int page, string sort, bool? descSort,
            DateTimeOffset? beginDate, DateTimeOffset? endDate, EntryType? entryType)
        {
            var filter = new Filter<DB.Context.Entry>()
            {
                BeginDate = beginDate ?? DateTimeOffset.Now,
                EndDate = endDate ?? DateTimeOffset.MaxValue,
                Page = page,
                Size = size,
                SortField = sort,
                DescSort = descSort
            };
            if (entryType != null)
            {
                filter.AddFilter = s => s.EntryType == entryType;
            }

            var entries = await repositoryAll.GetByFilter(filter);
            entries.BeginNumber = page * size + 1;
            entries.EndNumber = page * size + size;

            return mapper.Map<Models.FilteredList<AllEntry>>(entries);
        }

        public async Task<GroupedList<AllEntry>> GetDaily(int offset)
        {
            var now = DateTimeOffset.Now.AddDays(offset);
            DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);

            var filter = new Filter<DB.Context.Entry>()
            {
                BeginDate = beginDate,
                EndDate = beginDate.AddDays(1)
            };
            var entries = await repositoryAll.GetByFilter(filter);

            return new GroupedList<AllEntry>()
            {
                Entries = entries.Entries
                    .GroupBy(s => s.BeginDate.Hour.ToString())
                    .ToDictionary(s => s.Key, s => s.Select(c => mapper.Map<AllEntry>(c)))
            };
        }

        public async Task<GroupedList<AllEntry>> GetMonthly(int offset)
        {
            var now = DateTimeOffset.Now.AddMonths(offset);
            DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

            var filter = new Filter<DB.Context.Entry>()
            {
                BeginDate = beginDate,
                EndDate = beginDate.AddMonths(1)
            };
            var entries = await repositoryAll.GetByFilter(filter);

            return new GroupedList<AllEntry>()
            {
                Entries = entries.Entries
                    .GroupBy(s => s.BeginDate.Day.ToString())
                    .ToDictionary(s => s.Key, s => s.Select(c => mapper.Map<AllEntry>(c)))
            };
        }

        public async Task<GroupedList<AllEntry>> GetWeekly(int offset)
        {
            var now = DateTimeOffset.Now.AddDays(offset * 7).AddDays(-(int)DateTimeOffset.Now.DayOfWeek);
            DateTimeOffset beginDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);

            var filter = new Filter<DB.Context.Entry>()
            {
                BeginDate = beginDate,
                EndDate = beginDate.AddMonths(1)
            };
            var entries = await repositoryAll.GetByFilter(filter);

            return new GroupedList<AllEntry>()
            {
                Entries = entries.Entries
                    .GroupBy(s => s.BeginDate.Day.ToString())
                    .ToDictionary(s => s.Key, s => s.Select(c => mapper.Map<AllEntry>(c)))
            };
        }
    }

    public class TestDataService : IDataService
    {
        public TestDataService(IRepositoryAll repositoryAll, ILogger<DataService> logger)
        {

        }


        public async Task<Models.FilteredList<AllEntry>> GetList(int size, int page, string sort, bool? descSort,
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

    public class DataService<TEntry, TDbEntry> : IDataService<TEntry>
        where TEntry : Entry
        where TDbEntry : DB.Context.Entry
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

        public async Task<TEntry> AddItem(TEntry entry)
        {
            try
            {
                var entity = mapper.Map<TDbEntry>(entry);
                entity.Id = Guid.NewGuid();
                entity.VersionDate = DateTimeOffset.Now;
                var result = await repository.Add(entity);
                return mapper.Map<TEntry>(result);

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(AddItem)} : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<TEntry> DeleteItem(Guid id)
        {
            try
            {
                var result = await repository.Delete(id);
                return mapper.Map<TEntry>(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetItem)} : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<TEntry> GetItem(Guid id)
        {
            try
            {
                var result = await repository.Get(id);
                return mapper.Map<TEntry>(result);

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetItem)} : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<TEntry> UpdateItem(TEntry entry)
        {
            try
            {
                var result = await repository.Update(mapper.Map<TDbEntry>(entry));
                return mapper.Map<TEntry>(result);

            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(GetItem)} : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }

    public interface IDataService
    {
        Task<GroupedList<AllEntry>> GetDaily(int offset);

        Task<Models.FilteredList<AllEntry>> GetList(int size, int page, string sort, bool? descSort, DateTimeOffset? beginDate, DateTimeOffset? endDate, EntryType? entryType);
        Task<GroupedList<AllEntry>> GetMonthly(int offset);
        Task<GroupedList<AllEntry>> GetWeekly(int offset);
    }

    public interface IDataService<TEntry> where TEntry : Entry
    {
        Task<TEntry> AddItem(TEntry entry);
        Task<TEntry> DeleteItem(Guid id);
        Task<TEntry> GetItem(Guid id);
        Task<TEntry> UpdateItem(TEntry entry);
    }
}
