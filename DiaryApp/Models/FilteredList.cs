using Contracts;
using System.Collections.Generic;

namespace DiaryApp.Models
{
    public class FilteredList<T> : IFilteredList<T> where T : Entity
    {
        public IEnumerable<T> Entries { get; set; }
        public int AllCount { get; set; }
        public int BeginNumber { get; set; }
        public int EndNumber { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
    }
}
