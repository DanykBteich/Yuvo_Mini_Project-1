using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_Project.Data.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregatorController : ControllerBase
    {
        private static AggregatorService _aggregatorService;

        public AggregatorController(AggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService;
        }

        //[HttpGet("aggregate-data")]
        //public IActionResult AggregateData()
        //{
        //    try
        //    {
        //        //_aggregatorService.AggregateData();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        public static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            string fileName = Path.GetFileNameWithoutExtension(e.Name);
            _aggregatorService.AggregateData(fileName);
        }
    }
}
