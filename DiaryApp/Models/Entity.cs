using Contracts;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
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

    public abstract class Entity
    {
        public Guid Id { get; set; }
    }

    public abstract class Entry : Entity, IEntry
    {
        public EntryType EntryType { get; set; }
        public EntryTypeReference EntryTypeFull => EntryTypeReferences.GetReference(EntryType);
        public string Description { get; set; }
        public string Title { get; set; }
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
        public DateTimeOffset EndDate { get; set; }
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

    public class FilteredList<T> : IFilteredList<T> where T : Entry
    {
        public IEnumerable<T> Entries { get; set; }
        public int AllCount { get; set; }
        public int BeginNumber { get; set; }
        public int EndNumber { get; set; }
    }

    public class GroupedList<T>
    {
        public Dictionary<string, IEnumerable<T>> Entries { get; set; }
    }
}
