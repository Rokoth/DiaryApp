using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.Extensions.Logging;

namespace DiaryApp.Controllers
{
    public class MeetingController : EntryController<MeetingEntry>
    {
        public MeetingController(ILogger<MeetingController> logger, IDataService<MeetingEntry> dataService) : base(logger, dataService) { }
    }
}