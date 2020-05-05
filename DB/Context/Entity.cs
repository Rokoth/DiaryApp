using Contracts;
using System;

namespace DB.Context
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTimeOffset VersionDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class Entry : Entity, IEntry
    {
        public virtual EntryType EntryType { get; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTimeOffset BeginDate { get; set; }
    }

    public class MeetingEntry : Entry
    {
        public override EntryType EntryType => EntryType.Meeting;
        public DateTimeOffset EndDate { get; set; }
        public string Place => MeetingPlace.Place;

        public MeetingPlace MeetingPlace { get; set; }
    }

    public class MeetingPlace
    {
        public Guid Id { get; set; }
        public string Place { get; set; }
    }

    public class DealEntry : Entry
    {
        public override EntryType EntryType => EntryType.Deal;
        public DateTimeOffset EndDate { get; set; }
    }

    public class MemoEntry : Entry
    {
        public override EntryType EntryType => EntryType.Memo;
    }
}
