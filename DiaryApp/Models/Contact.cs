using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DiaryApp.Models
{
    public class Contact : Entity
    {
        [DisplayName("Имя")]
        [Required(ErrorMessage = "Не указано имя")]
        public string FirstName { get; set; }
        [DisplayName("Отчество")]
        [Required(ErrorMessage = "Не указано отчество")]
        public string SecondName { get; set; }
        [DisplayName("Фамилия")]
        [Required(ErrorMessage = "Не указана фамилия")]
        public string ThirdName { get; set; }
        [DisplayName("Дата рождения")]
        public DateTimeOffset BirthDate { get; set; }
        [DisplayName("Дата рождения")]
        public string BirthDateView => BirthDate.ToString("yyyy-MM-dd");
        [DisplayName("Компания")]
        public string Company { get; set; }
        [DisplayName("Должность")]
        public string Position { get; set; }
        [DisplayName("Контактная информация")]
        public string ContactInfoSummary => string.Join("\r\n", ContactInfos?.Select(s => s.ContactInfoTypeFull.Name + ": " + s.Value));

        public IEnumerable<ContactInfo> ContactInfos { get; set; }
    }
}
