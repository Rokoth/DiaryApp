using System.Collections.Generic;

namespace DiaryApp.Models
{
    public class GroupedList<T>
    {
        public string Key { get; set; }
        public Dictionary<string, IEnumerable<T>> Entries { get; set; }
    }
}
