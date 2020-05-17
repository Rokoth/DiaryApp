using Contracts;
using System.Linq;

namespace DiaryApp.Models
{
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
}
