using TechYardHub.BusinessLayer.Interfaces;
using TechYardHub.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechYardHub.Areas.Support.Controllers
{
    [Area("Support")]
    [Authorize(Policy = "Support Developer")]
    public class RequestResponseViewerController : Controller
    {
        private readonly IRequestResponseService _requestResponseService;

        public RequestResponseViewerController(IRequestResponseService requestResponseService)
        {
            _requestResponseService = requestResponseService;
        }
        [Authorize(Policy = "Support Developer")]
        public async Task<IActionResult> Index()
        {
            var logs = await _requestResponseService.GetAllLogsAsync();
            return View(logs);
        }

        public async Task<IActionResult> GetLogs()
        {
            var logs = await _requestResponseService.GetAllLogsAsync();
            var logDtos = logs.Select(log => new
            {
                log.Timestamp,
                log.RequestUrl,
                log.HttpMethod,
                log.RequestBody,
                ResponseBody = JsonFormatter.FormatJson(log.ResponseBody)
            });
            return Json(logDtos);
        }
    }
}