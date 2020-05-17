using Contracts;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DiaryApp.Models
{
    public class ContactInfo : Entity
    {
        public Guid ContactId { get; set; }
        [DisplayName("Тип контакта")]
        [Required(ErrorMessage = "Не указан тип контактной информации")]
        public ContactInfoType ContactInfoType { get; set; }
        public ContactInfoTypeReference ContactInfoTypeFull => ContactInfoTypeReferences.GetReference(ContactInfoType);
        [DisplayName("Значение")]
        [Required(ErrorMessage = "Не указано значение контакта")]
        public string Value { get; set; }
    }
}
