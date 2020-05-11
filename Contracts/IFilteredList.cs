using System.Collections.Generic;

namespace Contracts
{
    public interface IFilteredList<T>
    {
        int AllCount { get; set; }
        IEnumerable<T> Entries { get; set; }
    }
}