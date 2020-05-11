using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiaryApp.Controllers
{
    public abstract class EntryController<TEntry> : Controller where TEntry : Entry, new()
    {

        protected readonly ILogger<EntryController<TEntry>> _logger;
        protected readonly IDataService<TEntry> dataService;

        public EntryController(ILogger<EntryController<TEntry>> logger, IDataService<TEntry> dataService)
        {
            _logger = logger;
            this.dataService = dataService;
        }

        // GET: TEntry
        public async Task<ActionResult> Index(Guid id)
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

        // GET: TEntry/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TEntry/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TEntry entry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = await dataService.AddItem(entry);
                    return RedirectToAction(nameof(Index), new { id = item.Id });
                }
                else
                {
                    return View(entry);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: TEntry/Edit/5
        public async Task<ActionResult> Edit(Guid id)
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

        // POST: TEntry/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, IFormCollection collection)
        {
            try
            {
                TEntry entry = new TEntry
                {
                    Id = id
                };
                entry.SetValues(collection.ToDictionary(s => s.Key, s => s.Value));
                TEntry item = await dataService.UpdateItem(entry);
                return RedirectToAction(nameof(Index), new { id = item.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: TEntry/Delete/5
        public async Task<ActionResult> Delete(Guid id)
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

        // POST: TEntry/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                TEntry item = await dataService.DeleteItem(id);
                return RedirectToAction(nameof(Index), "Diary");
            }
            catch
            {
                return View();
            }
        }
    }
}