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

    public class ContactDataService : DataService<Models.Contact, DB.Context.Contact>, IContactDataService
    {
        private readonly IRepository<DB.Context.ContactInfo> repositoryInfo;
        public ContactDataService(
            IRepository<DB.Context.Contact> repository,
            IRepository<DB.Context.ContactInfo> repositoryInfo,
            ILogger<ContactDataService> logger,
            IMapper mapper) : base(repository, logger, mapper)
        {
            this.repositoryInfo = repositoryInfo;
        }

        public async Task<Models.FilteredList<Contact>> GetList(
            int size, int page, string sort, bool? descSort,
            string search, DateTimeOffset? birthDateBegin, DateTimeOffset? birthDateEnd)
        {
            var filter = new Filter<DB.Context.Contact>()
            {
                Page = page,
                Size = size,
                SortField = sort,
                DescSort = descSort,
                AddFilter = s =>
                    (birthDateBegin == null || s.BirthDate >= birthDateBegin)
                    && (birthDateEnd == null || s.BirthDate < birthDateEnd)
                    && (string.IsNullOrEmpty(search)
                        || s.FirstName.Contains(search)
                        || s.SecondName.Contains(search)
                        || s.ThirdName.Contains(search)
                        || s.Company.Contains(search)
                        || s.Position.Contains(search)
                        || s.ContactInfos.Any(c => c.Value.Contains(search)))
            };
            var entries = await repository.GetByFilter(filter, "ContactInfos");
            entries.BeginNumber = page * size + 1;
            entries.EndNumber = page * size + size;

            return mapper.Map<Models.FilteredList<Contact>>(entries);
        }

        protected override async Task<Models.Contact> UpdateInternal(Models.Contact entry)
        {
            var result = await repository.Update(mapper.Map<DB.Context.Contact>(entry), false);
            var info = await repositoryInfo.GetByFilter(new Filter<DB.Context.ContactInfo>()
            {
                AddFilter = s => s.ContactId == entry.Id
            });
            if (entry.ContactInfos != null)
            {
                var newItems = entry.ContactInfos.Where(s => !info.Entries
                    .Any(c => s.ContactInfoType == c.ContactInfoType && s.Value == c.Value));

                foreach (var infoItem in newItems)
                {
                    var newInfo = mapper.Map<DB.Context.ContactInfo>(infoItem);
                    newInfo.ContactId = entry.Id;
                    newInfo.Id = Guid.NewGuid();
                    await repositoryInfo.Add(newInfo, false);
                }

                var toDelItems = info.Entries.Where(s => !entry.ContactInfos
                    .Any(c => s.ContactInfoType == c.ContactInfoType && s.Value == c.Value));

                foreach (var infoItem in toDelItems)
                {
                    await repositoryInfo.Delete(infoItem.Id, false);
                }
            }
            else
            {
                foreach (var infoItem in info.Entries)
                {
                    await repositoryInfo.Delete(infoItem.Id, false);
                }
            }

            await repository.SaveChangesAsync();
            return mapper.Map<Models.Contact>(result);
        }

        protected override async Task<Models.Contact> GetItemInternal(Guid id)
        {
            var result = await repository.Get(id, "ContactInfos");
            return mapper.Map<Models.Contact>(result);
        }

        protected override async Task<Models.Contact> DeleteInternal(Guid id)
        {
            var result = await repository.Delete(id, false);
            var toDelItems = await repositoryInfo.GetByFilter(new Filter<DB.Context.ContactInfo>()
            {
                AddFilter = s => s.ContactId == id
            });
            foreach (var infoItem in toDelItems.Entries)
            {
                await repositoryInfo.Delete(infoItem.Id, false);
            }
            await repository.SaveChangesAsync();
            return mapper.Map<Models.Contact>(result);
        }

        protected override async Task<Models.Contact> AddItemInternal(Models.Contact entry)
        {
            var entity = mapper.Map<DB.Context.Contact>(entry);
            entity.Id = Guid.NewGuid();
            entity.VersionDate = DateTimeOffset.Now;
            var result = await repository.Add(entity, false);

            foreach (var infoItem in entry.ContactInfos)
            {
                var newInfo = mapper.Map<DB.Context.ContactInfo>(infoItem);
                newInfo.ContactId = entry.Id;
                newInfo.Id = Guid.NewGuid();
                await repositoryInfo.Add(newInfo, false);
            }

            await repository.SaveChangesAsync();
            return mapper.Map<Models.Contact>(result);
        }
    }

    public class MeetingDataService : DataService<Models.MeetingEntry, DB.Context.MeetingEntry>
    {
        private readonly IRepository<DB.Context.MeetingPlace> repositoryPlace;
        public MeetingDataService(
            IRepository<DB.Context.MeetingEntry> repository,
            IRepository<DB.Context.MeetingPlace> repositoryPlace,
            ILogger<MeetingDataService> logger,
            IMapper mapper) : base(repository, logger, mapper)
        {
            this.repositoryPlace = repositoryPlace;
        }

        protected override async Task<Models.MeetingEntry> UpdateInternal(Models.MeetingEntry entry)
        {
            var result = await repository.Update(mapper.Map<DB.Context.MeetingEntry>(entry), false);
            var place = await repositoryPlace.Get(entry.Id);
            place.Place = entry.Place;
            await repositoryPlace.Update(mapper.Map<DB.Context.MeetingPlace>(place), false);
            await repository.SaveChangesAsync();
            return mapper.Map<Models.MeetingEntry>(result);
        }

        protected override async Task<Models.MeetingEntry> GetItemInternal(Guid id)
        {
            var result = await repository.Get(id, "MeetingPlace");
            return mapper.Map<Models.MeetingEntry>(result);
        }

        protected override async Task<Models.MeetingEntry> DeleteInternal(Guid id)
        {
            var result = await repository.Delete(id, false);
            await repositoryPlace.Delete(id, false);
            await repository.SaveChangesAsync();
            return mapper.Map<Models.MeetingEntry>(result);
        }

        protected override async Task<Models.MeetingEntry> AddItemInternal(Models.MeetingEntry entry)
        {
            var entity = mapper.Map<DB.Context.MeetingEntry>(entry);
            entity.Id = Guid.NewGuid();
            entity.VersionDate = DateTimeOffset.Now;
            var result = await repository.Add(entity, false);
            var place = new DB.Context.MeetingPlace()
            {
                Id = entity.Id,
                VersionDate = DateTimeOffset.Now,
                Place = entity.Place
            };
            await repositoryPlace.Add(place, false);
            await repository.SaveChangesAsync();
            return mapper.Map<Models.MeetingEntry>(result);
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

    public interface IDataService
    {
        Task<GroupedList<AllEntry>> GetDaily(int offset);

        Task<Models.FilteredList<AllEntry>> GetList(int size, int page, string sort, bool? descSort, string search, DateTimeOffset? beginDate, DateTimeOffset? endDate, EntryType? entryType);
        Task<GroupedList<AllEntry>> GetMonthly(int offset);
        Task<GroupedList<AllEntry>> GetWeekly(int offset);
    }

    public interface IContactDataService : IDataService<Models.Contact>
    {
        Task<Models.FilteredList<Models.Contact>> GetList(int size, int page, string sort, bool? descSort,
            string search, DateTimeOffset? birthDateBegin, DateTimeOffset? birthDateEnd);
    }

    public interface IDataService<TEntry> where TEntry : Models.Entity
    {
        Task<TEntry> AddItem(TEntry entry);
        Task<TEntry> DeleteItem(Guid id);
        Task<TEntry> GetItem(Guid id);
        Task<TEntry> UpdateItem(TEntry entry);
    }
}
