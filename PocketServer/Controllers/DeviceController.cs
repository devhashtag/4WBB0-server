using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using PocketServer.DataAccess.Entities;
using PocketServer.DataAccess.Services;

namespace PocketServer.Controllers
{
    public class Location 
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }

    [ApiController]
    [Route("/[controller]/[action]")]
    public class DeviceController : Controller
    {
        private DeviceService _deviceService;
        private HeartbeatService _heartbeatService;

        public DeviceController(DeviceService deviceService, HeartbeatService heartbeatService)
        {
            _deviceService = deviceService;
            _heartbeatService = heartbeatService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello");
        }

        [HttpPost("{deviceId}")]
        public async Task<IActionResult> Heartbeat([FromRoute] string deviceId, [FromBody] Location location)
        {
            if (location == null)
            {
                return BadRequest();
            }

            if (!_deviceService.Exists(deviceId))
            {
                return BadRequest();
            }

            var heartbeat = new Heartbeat()
            {
                DeviceId = deviceId,
                Longitude = location.Long,
                Lattitue = location.Lat
            };

            await _heartbeatService.AddAsync(heartbeat);

            return Ok();
        }

        [HttpPost("{deviceId}")]
        public async Task<IActionResult> Alert([FromRoute] string deviceId)
        {
            return Ok();
        }

        [HttpPost("{deviceId}")]
        public async Task<IActionResult> Cancel([FromRoute] string deviceId)
        {
            return Ok();
        }
    }
}
