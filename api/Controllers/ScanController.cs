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
    /// <summary>
    /// Controlleur pour gérer le scan
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
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


        /// <summary>
        /// Retourne la liste de tous les scans éxécutés
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("")]
        public IActionResult GetAll()
        {
            return Json(_scanService.GetAllScanResultEntities());
        }

        /// <summary>
        /// Ajouter un scan à la liste
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.InvalidDataException">The modal is invalid</exception>
        [HttpPost]
        [ActionName("add")]
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

        /// <summary>
        /// Informe qu'une intrusion a eu lieu
        /// </summary>
        /// <returns></returns>
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
