using Contracts;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DiaryApp.Models
{
    public abstract class Entry : Entity, IEntry
    {
        [DisplayName("Тип записи")]
        public EntryType EntryType { get; set; }
        public EntryTypeReference EntryTypeFull => EntryTypeReferences.GetReference(EntryType);
        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Заголовок")]
        [Required(ErrorMessage = "Не указан заголовок")]
        public string Title { get; set; }

        [DisplayName("Дата начала")]
        [Required(ErrorMessage = "Не указана дата начала события")]
        public DateTimeOffset BeginDate { get; set; }

        public virtual void SetValues(Dictionary<string, StringValues> values)
        {
            if (values.ContainsKey("Description")) Description = values["Description"];
            if (values.ContainsKey("Title")) Title = values["Title"];
            if (values.ContainsKey("BeginDate")) BeginDate = DateTimeOffset.Parse(values["BeginDate"]);
        }
    }
}
