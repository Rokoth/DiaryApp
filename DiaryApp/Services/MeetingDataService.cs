using AutoMapper;
using DB.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DiaryApp.Services
{
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
}
