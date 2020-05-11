using Contracts;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DiaryApp.Models
{
    public class EntryTypeReference
    {
        public EntryType EntryType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public static class EntryTypeReferences
    {
        private static EntryTypeReference[] References => new EntryTypeReference[] {
            new EntryTypeReference()
            {
                EntryType = EntryType.All,
                Code = nameof(EntryType.All),
                Name = "Не определено"
            },
            new EntryTypeReference()
            {
                EntryType = EntryType.Deal,
                Code = nameof(EntryType.Deal),
                Name = "Дело"
            },
            new EntryTypeReference()
            {
                EntryType = EntryType.Meeting,
                Code = nameof(EntryType.Meeting),
                Name = "Встреча"
            },
            new EntryTypeReference()
            {
                EntryType = EntryType.Memo,
                Code = nameof(EntryType.Memo),
                Name = "Заметка"
            }
        };

        public static EntryTypeReference GetReference(EntryType entryType)
        {
            return References.FirstOrDefault(s => s.EntryType == entryType);
        }
    }

    public class ContactInfoTypeReference
    {
        public ContactInfoType EntryType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

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

        public static ContactInfoTypeReference GetReference(ContactInfoType entryType)
        {
            return References.FirstOrDefault(s => s.EntryType == entryType);
        }
    }

    public class Contact : Entity
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }

        public IEnumerable<ContactInfo> ContactInfos { get; set; }
    }

    public class ContactInfo : Entity
    {
        public Guid ContactId { get; set; }
        public ContactInfoType ContactInfoType { get; set; }
        public ContactInfoTypeReference ContactInfoTypeFull => ContactInfoTypeReferences.GetReference(ContactInfoType);
        public string Value { get; set; }
    }

    public abstract class Entity
    {
        public Guid Id { get; set; }
    }

    public abstract class Entry : Entity, IEntry
    {
        [DisplayName("Тип записи")]
        public EntryType EntryType { get; set; }
        public EntryTypeReference EntryTypeFull => EntryTypeReferences.GetReference(EntryType);
        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Заголовок")]
        [Required(ErrorMessage = "Не указан заголовок")]
        public string Title { get; set; }

        [DisplayName("Дата начала")]
        [Required(ErrorMessage = "Не указана дата начала события")]
        public DateTimeOffset BeginDate { get; set; }

        public virtual void SetValues(Dictionary<string, StringValues> values)
        {
            if (values.ContainsKey("Description")) Description = values["Description"];
            if (values.ContainsKey("Title")) Title = values["Title"];
            if (values.ContainsKey("BeginDate")) BeginDate = DateTimeOffset.Parse(values["BeginDate"]);
        }
    }

    public class AllEntry : Entry
    {
    }

    public class MeetingEntry : Entry
    {
        [DisplayName("Дата окончания")]
        [Required(ErrorMessage = "Не указана дата окончания события")]
        public DateTimeOffset EndDate { get; set; }
        [DisplayName("Место")]
        [Required(ErrorMessage = "Не указано место события")]
        public string Place { get; set; }

        public override void SetValues(Dictionary<string, StringValues> values)
        {
            base.SetValues(values);
            if (values.ContainsKey("Place")) Place = values["Place"];
            if (values.ContainsKey("EndDate")) EndDate = DateTimeOffset.Parse(values["EndDate"]);
        }
    }

    public class DealEntry : Entry
    {
        [DisplayName("Дата окончания")]
        [Required(ErrorMessage = "Не указана дата окончания события")]
        public DateTimeOffset EndDate { get; set; }
        public override void SetValues(Dictionary<string, StringValues> values)
        {
            base.SetValues(values);
            if (values.ContainsKey("EndDate")) EndDate = DateTimeOffset.Parse(values["EndDate"]);
        }
    }

    public class MemoEntry : Entry
    {
    }

    public class FilteredList<T> : IFilteredList<T> where T : Entity
    {
        public IEnumerable<T> Entries { get; set; }
        public int AllCount { get; set; }
        public int BeginNumber { get; set; }
        public int EndNumber { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
    }

    public class GroupedList<T>
    {
        public string Key { get; set; }
        public Dictionary<string, IEnumerable<T>> Entries { get; set; }
    }
}
