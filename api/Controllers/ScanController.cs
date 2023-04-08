using Microsoft.AspNetCore.Mvc;
using RoomScannerWeb.Data;
using RoomScannerWeb.Data.Models;

namespace RoomScannerWeb.Controllers
{
    [ApiController]
    [Route("api/scans/[Action]")]
    public class ScanController : Controller
    {
        private readonly IScanService _scanService;

        public ScanController(IScanService scanService)
        {
            _scanService = scanService;
        }
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("hello-world")]
        public IActionResult Index()
        {
            return Json(new { message = "Hello from ScanController" });
        }

        [HttpPost]
        [ActionName("")]
        public IActionResult AppendScanResult(ScanResultPost model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ScanResultEntity entity = new ScanResultEntity(model);

                    _scanService.InsertScanResult(entity);

                    return Ok("Scan result added");
                }
                else
                {
                    throw new InvalidDataException("The modal is invalid");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
