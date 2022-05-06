using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Mini_Project.Data.Models;
using Mini_Project.Data.Services;
using System;

namespace Mini_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UIValuesController : ControllerBase
    {
        public UIValuesService _uIValuesService;

        public UIValuesController(UIValuesService uIValuesService)
        {
            _uIValuesService = uIValuesService;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("/{aggType}")]
        public IActionResult GetDailyData(string aggType, string dateTimeFrom, string datetimeTo)
        {
            try
            {
                var _result = _uIValuesService.GetData(aggType, dateTimeFrom, datetimeTo);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[EnableCors("CorsPolicy")]
        ////[HttpPost]
        //[HttpGet("getUIValues")]
        ////[Route ("getUIValues")]
        //public IActionResult GetDailyData([FromBody] UIValuesParams uiValues)
        //{
        //    try
        //    {
        //        var _result = _uIValuesService.GetData(uiValues.aggType, uiValues.dateTimeFrom, uiValues.datetimeTo);
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
