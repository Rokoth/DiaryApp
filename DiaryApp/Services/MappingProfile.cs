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
            CreateMap<DB.Repository.FilteredList<DB.Context.Contact>, Models.FilteredList<Contact>>();


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

            CreateMap<DB.Context.Contact, Contact>();
            CreateMap<Contact, DB.Context.Contact>();

            CreateMap<DB.Context.ContactInfo, ContactInfo>();
            CreateMap<ContactInfo, DB.Context.ContactInfo>();
        }
    }
}
