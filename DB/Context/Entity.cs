using Contracts;
using System;
using System.Collections.Generic;

namespace DB.Context
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTimeOffset VersionDate { get; set; }
        public bool IsDeleted { get; set; }
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
        public string Value { get; set; }
    }

    public class Entry : Entity, IEntry
    {
        public virtual EntryType EntryType { get; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTimeOffset BeginDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }

    public class MeetingEntry : Entry
    {
        public override EntryType EntryType => EntryType.Meeting;
        public string Place => MeetingPlace.Place;
        public MeetingPlace MeetingPlace { get; set; }
    }

    public class MeetingPlace : Entity
    {
        public string Place { get; set; }
    }

    public class DealEntry : Entry
    {
        public override EntryType EntryType => EntryType.Deal;
    }

    public class MemoEntry : Entry
    {
        public override EntryType EntryType => EntryType.Memo;
    }
}
