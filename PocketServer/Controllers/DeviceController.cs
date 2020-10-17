using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using PocketServer.DataAccess.Entities;
using PocketServer.DataAccess.Services;
using PocketServer.WebSockets;

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
        private readonly DeviceService _deviceService;
        private readonly HeartbeatService _heartbeatService;
        private readonly AlertService _alertService;
        private readonly WebSocketHandler _websocketHandler;

        public DeviceController(WebSocketHandler websocketHandler, DeviceService deviceService, HeartbeatService heartbeatService, AlertService alertService)
        {
            _deviceService = deviceService;
            _heartbeatService = heartbeatService;
            _alertService = alertService;
            _websocketHandler = websocketHandler;
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

            // May return information about medicine reminder
            return Ok();
        }

        [HttpGet("{deviceId}")]
        public async Task<IActionResult> Alert([FromRoute] string deviceId)
        {
            if (! await _deviceService.ExistsAsync(deviceId))
            {
                return BadRequest("Device does not exist"); 
            }

            var alert = new Alert();

            alert.DeviceId = deviceId;
            alert.Cancelled = false;

            var lastHeartbeat = _heartbeatService.GetAll()
                .Where(h => h.DeviceId == deviceId)
                .OrderByDescending(h => h.Timestamp)
                .FirstOrDefault();

            //var lastHeartbeat = (await _deviceService.GetByIdAsync(deviceId))
            //    .Heartbeats
            //    .OrderByDescending(h => h.Timestamp)
            //    .FirstOrDefault();

            if (lastHeartbeat != null)
            {
                alert.LocationLat = lastHeartbeat.Lattitue;
                alert.LocationLong = lastHeartbeat.Longitude;
            }

            await _alertService.AddAsync(alert);

            // Send alert to all connected users
            
            return Ok();
        }

        [HttpGet("{deviceId}")]
        public async Task<IActionResult> Cancel([FromRoute] string deviceId)
        {
            if (! await _deviceService.ExistsAsync(deviceId))
            {
                return BadRequest("Device does not exist");
            }

            var lastAlert = _alertService.GetAll()
                .Where(a => a.DeviceId == deviceId)
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefault();

            //var lastAlert = (await _deviceService.GetByIdAsync(deviceId))
            //    .Alerts
            //    .Where(a => a.Cancelled == false)
            //    .OrderByDescending(a => a.Timestamp)
            //    .FirstOrDefault();

            if (lastAlert == null || lastAlert.Cancelled == true)
            {
                return BadRequest("There is no previous non-cancelled alert");
            }

            lastAlert.Cancelled = true;
            await _alertService.UpdateAsync(lastAlert);

            return Ok();
        }
    }
}
