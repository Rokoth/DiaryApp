﻿using Contracts;
using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiaryApp.Controllers
{
    public class DiaryController : Controller
    {
        private readonly ILogger<DiaryController> _logger;
        private readonly IDataService dataService;

        public DiaryController(ILogger<DiaryController> logger, IDataService dataService)
        {
            _logger = logger;
            this.dataService = dataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(
            [FromQuery]int size = 10,
            [FromQuery]int page = 0,
            [FromQuery]string sort = null,
            [FromQuery]bool? descSort = null,
            [FromQuery]string search = null,
            [FromQuery]DateTimeOffset? beginDate = null,
            [FromQuery]DateTimeOffset? endDate = null,
            [FromQuery]EntryType? entryType = null)
        {
            try
            {
                if (string.IsNullOrEmpty(sort))
                {
                    sort = "BeginDate";
                    descSort = true;
                }
                var entries = await dataService.GetList(size, page, sort, descSort, search, beginDate, endDate, entryType);
                SetHeaders(entries.AllCount, entries.BeginNumber, entries.EndNumber, entries.IsFirstPage, entries.IsLastPage);
                return PartialView(entries.Entries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GET List method: {ex.Message}, StackTrace: {ex.StackTrace}");
                return BadRequest(ex.Message);
            }
        }


        public async Task<IActionResult> Daily(int offset = 0)
        {
            try
            {
                var entries = await dataService.GetDaily(offset);
                SetHeaders(entries.Entries.Count, 1, entries.Entries.Count, false, false);
                return PartialView(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GET Daily method: {ex.Message}, StackTrace: {ex.StackTrace}");
                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> Monthly(int offset = 0)
        {
            try
            {
                var entries = await dataService.GetMonthly(offset);
                SetHeaders(entries.Entries.Count, 1, entries.Entries.Count, false, false);
                return PartialView(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GET Monthly method: {ex.Message}, StackTrace: {ex.StackTrace}");
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> Weekly(int offset = 0)
        {
            try
            {
                var entries = await dataService.GetWeekly(offset);
                SetHeaders(entries.Entries.Count, 1, entries.Entries.Count, false, false);
                return PartialView(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GET Weekly method: {ex.Message}, StackTrace: {ex.StackTrace}");
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetHeaders(int allCount, int beginNumber, int endNumber, bool isFirstPage, bool isLastPage)
        {
            HttpContext.Response.Headers.Add("X-Header-AllCount", allCount.ToString());
            HttpContext.Response.Headers.Add("X-Header-BeginNumber", beginNumber.ToString());
            HttpContext.Response.Headers.Add("X-Header-EndNumber", endNumber.ToString());
            HttpContext.Response.Headers.Add("X-Header-IsFirstPage", isFirstPage.ToString());
            HttpContext.Response.Headers.Add("X-Header-IsLastPage", isLastPage.ToString());
        }
    }
}
