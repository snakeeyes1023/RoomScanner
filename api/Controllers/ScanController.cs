using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoomScannerWeb.ActionFilters;
using RoomScannerWeb.Data;
using RoomScannerWeb.Data.Models;

namespace RoomScannerWeb.Controllers
{
    [ApiController]
    [Route("api/scans/[Action]")]
    [TypeFilter(typeof(IPValidationActionFilter))]
    public class ScanController : Controller
    {
        private readonly IScanService _scanService;

        public ScanController(IScanService scanService)
        {
            _scanService = scanService;
        }

        [HttpGet]
        [ActionName("")]
        public IActionResult GetAll()
        {
            return Json(_scanService.GetAllEntities());
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
