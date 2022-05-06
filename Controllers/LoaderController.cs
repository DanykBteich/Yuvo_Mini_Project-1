using Microsoft.AspNetCore.Mvc;
using Mini_Project.Data.Services;
using System;
using System.IO;

namespace Mini_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaderController : ControllerBase
    {
        private static LoaderService _loaderService;

        public LoaderController(LoaderService loaderService)
        {
            _loaderService = loaderService;
        }

        [HttpGet("load-data-to-vertica")]
        public IActionResult LoadToVertica()
        {
            try
            {
                _loaderService.LoadToVertica();
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

            _loaderService.LoadToVertica();
        }
    }
}
