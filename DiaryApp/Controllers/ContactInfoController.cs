using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DiaryApp.Controllers
{
    public class ContactInfoController : Controller
    {
        private readonly ILogger<ContactInfoController> _logger;
        private readonly IContactDataService dataService;
        public ContactInfoController(ILogger<ContactInfoController> logger, IContactDataService dataService)
        {
            _logger = logger;
            this.dataService = dataService;
        }


        // GET: ContactInfo/Details/5
        public async Task<IActionResult> Index(Guid id)
        {
            ContactInfo item = await dataService.GetContactInfoItem(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return BadRequest("Запись не найдена");
            }
        }

        // GET: ContactInfo/Create
        public IActionResult Create(Guid contactId)
        {
            return View(new ContactInfo() { ContactId = contactId });
        }

        // POST: ContactInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactInfo entry)
        {
            try
            {
                ContactInfo item = await dataService.AddContactInfoItem(entry);
                return RedirectToAction("Details", "Contact", new { id = item.ContactId });
            }
            catch
            {
                return View();
            }
        }

        // GET: ContactInfo/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await dataService.GetContactInfoItem(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return BadRequest("Запись не найдена");
            }
        }

        // POST: ContactInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactInfo entry)
        {
            try
            {
                var item = await dataService.UpdateContactInfoItem(entry);
                if (item != null)
                {
                    return RedirectToAction("Details", "Contact", new { id = item.ContactId });
                }
                else
                {
                    return BadRequest("Не удалось сохранить запись");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: ContactInfo/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await dataService.GetContactInfoItem(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return BadRequest("Запись не найдена");
            }
        }

        // POST: ContactInfo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, ContactInfo entry)
        {
            try
            {
                var item = await dataService.DeleteContactInfoItem(id);
                if (item != null)
                {
                    return RedirectToAction("Details", "Contact", new { id = item.ContactId });
                }
                else
                {
                    return BadRequest("Не удалось удалить запись");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}