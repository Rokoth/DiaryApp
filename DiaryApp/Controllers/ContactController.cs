using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DiaryApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactDataService dataService;
        public ContactController(ILogger<ContactController> logger, IContactDataService dataService)
        {
            _logger = logger;
            this.dataService = dataService;
        }
        public async Task<IActionResult> Index(
            [FromQuery]int size = 10,
            [FromQuery]int page = 0,
            [FromQuery]string sort = null,
            [FromQuery]bool? descSort = null,
            [FromQuery]string search = null,
            [FromQuery]DateTimeOffset? beginDate = null,
            [FromQuery]DateTimeOffset? endDate = null)
        {
            try
            {
                var entries = await dataService.GetList(size, page, sort, descSort, search, beginDate, endDate);
                return View(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GET List method: {ex.Message}, StackTrace: {ex.StackTrace}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Contact/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var item = await dataService.GetItem(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return BadRequest("Запись не найдена");
            }
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                Contact entry = new Contact()
                {
                    BirthDate = DateTimeOffset.Parse(collection["BirthDate"]),
                    Company = collection["Company"],
                    FirstName = collection["FirstName"],
                    Position = collection["Position"],
                    SecondName = collection["SecondName"],
                    ThirdName = collection["ThirdName"]
                };
                var item = await dataService.AddItem(entry);
                return RedirectToAction(nameof(Details), new { id = item.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await dataService.GetItem(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return BadRequest("Запись не найдена");
            }
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                Contact entry = new Contact()
                {
                    BirthDate = DateTimeOffset.Parse(collection["BirthDate"]),
                    Company = collection["Company"],
                    FirstName = collection["FirstName"],
                    Position = collection["Position"],
                    SecondName = collection["SecondName"],
                    ThirdName = collection["ThirdName"]
                };
                var item = await dataService.UpdateItem(entry);
                return RedirectToAction(nameof(Details), new { id = item.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await dataService.GetItem(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return BadRequest("Запись не найдена");
            }
        }

        // POST: Contact/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var item = await dataService.DeleteItem(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}