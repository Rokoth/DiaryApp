using System.Collections.Generic;

namespace Contracts
{
    public interface IFilteredList<T> where T : IEntry
    {
        int AllCount { get; set; }
        IEnumerable<T> Entries { get; set; }
    }
}