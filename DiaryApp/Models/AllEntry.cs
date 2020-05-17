using System;
using System.ComponentModel;

namespace DiaryApp.Models
{
    public class AllEntry : Entry
    {

        [DisplayName("Дата окончания")]
        public DateTimeOffset? EndDate { get; set; }
    }
}
