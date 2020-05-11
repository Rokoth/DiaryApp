using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.Extensions.Logging;

namespace DiaryApp.Controllers
{

    public class DealController : EntryController<DealEntry>
    {
        public DealController(ILogger<DealController> logger, IDataService<DealEntry> dataService) : base(logger, dataService) { }
    }
}