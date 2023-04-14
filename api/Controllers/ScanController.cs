using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using RoomScannerWeb.ActionFilters;
using RoomScannerWeb.Data.Entitites;
using RoomScannerWeb.Data.Helpers;
using RoomScannerWeb.Data.Models;
using RoomScannerWeb.Data.Services;

namespace RoomScannerWeb.Controllers
{
    [ApiController]
    [Route("api/scans/[Action]")]
    [TypeFilter(typeof(IPValidationActionFilter))]
    public class ScanController : Controller
    {
        private readonly IScanService _scanService;
        private readonly ScanSetting _scanSetting;

        public ScanController(IScanService scanService, IOptions<ScanSetting> scanSetting)
        {
            _scanService = scanService;
            _scanSetting = scanSetting.Value;
        }

        [HttpGet]
        [ActionName("")]
        public IActionResult GetAll()
        {
            return Json(_scanService.GetAllScanResultEntities());
        }

        [HttpPost]
        [ActionName("")]
        public IActionResult AppendScanResult(ScanResultPost model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmpty = (model.MaximalVariationDistance < _scanSetting.VariationOffset);

                    ScanResultEntity entity = new ScanResultEntity(model, isEmpty);

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

        [HttpPost]
        [ActionName("infiltrations")]
        public IActionResult AppendScanInfiltration()
        {
            try
            {
                _scanService.InsertIntrusion(new ScanIntrusionEntity(DateTime.Now));
                return Ok("Infiltration saved");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
