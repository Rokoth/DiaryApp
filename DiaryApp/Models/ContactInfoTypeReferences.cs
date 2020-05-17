using Contracts;
using System.Linq;

namespace DiaryApp.Models
{
    public static class ContactInfoTypeReferences
    {
        private static ContactInfoTypeReference[] References => new ContactInfoTypeReference[] {
            new ContactInfoTypeReference()
            {
                EntryType = ContactInfoType.Phone,
                Code = nameof(ContactInfoType.Phone),
                Name = "Телефон"
            },
            new ContactInfoTypeReference()
            {
                EntryType = ContactInfoType.Email,
                Code = nameof(ContactInfoType.Email),
                Name = "Email"
            },
            new ContactInfoTypeReference()
            {
                EntryType = ContactInfoType.Skype,
                Code = nameof(ContactInfoType.Skype),
                Name = "Skype"
            },
            new ContactInfoTypeReference()
            {
                EntryType = ContactInfoType.Other,
                Code = nameof(ContactInfoType.Other),
                Name = "Другое"
            },
        };

        public static ContactInfoTypeReference[] AllReferences => References;

        public static ContactInfoTypeReference GetReference(ContactInfoType entryType)
        {
            return References.FirstOrDefault(s => s.EntryType == entryType);
        }
    }
}
