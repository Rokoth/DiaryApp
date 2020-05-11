using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.Extensions.Logging;

namespace DiaryApp.Controllers
{
    public class MemoController : EntryController<MemoEntry>
    {
        public MemoController(ILogger<MemoController> logger, IDataService<MemoEntry> dataService) : base(logger, dataService) { }
    }
}