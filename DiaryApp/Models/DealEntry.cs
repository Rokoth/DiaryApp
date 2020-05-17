using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DiaryApp.Models
{
    public class DealEntry : Entry
    {
        [DisplayName("Дата окончания")]
        [Required(ErrorMessage = "Не указана дата окончания события")]
        public DateTimeOffset EndDate { get; set; }
        public override void SetValues(Dictionary<string, StringValues> values)
        {
            base.SetValues(values);
            if (values.ContainsKey("EndDate")) EndDate = DateTimeOffset.Parse(values["EndDate"]);
        }
    }
}
