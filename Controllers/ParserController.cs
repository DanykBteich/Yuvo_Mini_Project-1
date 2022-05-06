using Microsoft.AspNetCore.Mvc;
using Mini_Project.Data.Services;
using System;
using System.IO;

namespace Mini_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private static ParserService _parserService;

        public ParserController(ParserService parserService)
        {
            _parserService = parserService;
        }

        [HttpGet("parse-to-csv-file")]
        public IActionResult ParseToCSV()
        {
            try
            {
                _parserService.ParseToCSVFile();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            _parserService.ParseToCSVFile();
        }
    }
}
