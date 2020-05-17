using AutoMapper;
using DB.Repository;
using DiaryApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
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
            var result = mapper.Map<Models.FilteredList<Contact>>(entries);
            result.BeginNumber = page * size + 1;
            result.EndNumber = page * size + size;
            result.IsFirstPage = page == 0;
            result.IsLastPage = page * size + size >= entries.AllCount;

            return result;
        }

        protected override async Task<Models.Contact> UpdateInternal(Models.Contact entry)
        {
            var result = await repository.Update(mapper.Map<DB.Context.Contact>(entry));
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
            return mapper.Map<Models.Contact>(await repository.Add(entity));
        }

        public async Task<ContactInfo> AddContactInfoItem(ContactInfo entry)
        {
            try
            {
                var entity = mapper.Map<DB.Context.ContactInfo>(entry);
                entity.Id = Guid.NewGuid();
                entity.VersionDate = DateTimeOffset.Now;
                var result = await repositoryInfo.Add(entity);
                return mapper.Map<Models.ContactInfo>(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AddContactInfoItem : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<ContactInfo> DeleteContactInfoItem(Guid id)
        {
            try
            {
                var result = await repositoryInfo.Delete(id);
                return mapper.Map<Models.ContactInfo>(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DeleteContactInfoItem : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<ContactInfo> GetContactInfoItem(Guid id)
        {
            try
            {
                var result = await repositoryInfo.Get(id);
                return mapper.Map<Models.ContactInfo>(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in GetContactInfoItem : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<ContactInfo> UpdateContactInfoItem(ContactInfo entry)
        {
            try
            {
                var result = await repositoryInfo.Update(mapper.Map<DB.Context.ContactInfo>(entry));
                return mapper.Map<Models.ContactInfo>(result);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UpdateContactInfoItem : {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
