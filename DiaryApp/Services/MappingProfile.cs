using AutoMapper;
using DiaryApp.Models;

namespace DiaryApp.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DB.Context.Entry, AllEntry>();
            CreateMap<DB.Context.DealEntry, Models.DealEntry>();
            CreateMap<DB.Context.MeetingEntry, Models.MeetingEntry>();
            CreateMap<DB.Context.MemoEntry, Models.MemoEntry>();
            CreateMap<DB.Repository.FilteredList<DB.Context.Entry>, Models.FilteredList<AllEntry>>();


            CreateMap<Models.DealEntry, DB.Context.DealEntry>();
            CreateMap<Models.MeetingEntry, DB.Context.MeetingEntry>()
                .AfterMap((src, dst) =>
                {
                    dst.MeetingPlace = new DB.Context.MeetingPlace()
                    {
                        Id = dst.Id,
                        Place = src.Place
                    };
                });
            CreateMap<Models.MemoEntry, DB.Context.MemoEntry>();
        }
    }
}
