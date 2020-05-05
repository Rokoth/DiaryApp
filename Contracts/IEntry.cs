using System;

namespace Contracts
{
    public interface IEntry
    {
        DateTimeOffset BeginDate { get; set; }
        string Description { get; set; }
        string Title { get; set; }
        EntryType EntryType { get; }
    }

    public enum EntryType
    {
        All = 0,
        Meeting = 1,
        Deal = 2,
        Memo = 3
    }
}