using DisasterAlarm.service.Dtos.Request;
using DisasterAlarm.service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterAlarm.api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly AppService _service;
        public AppController(AppService service)
        {
            _service = service;
        }

        [HttpPost("regions")]
        public async Task<IActionResult> Regions(CreateRegionRequest[] request)
        {
            var resp = await _service.CreateRegionAsync(request);
            if(!resp.Success)
                return StatusCode(500,resp);
            return Ok(resp);
        }

        [HttpPost("alert-settings")]
        public async Task<IActionResult> AlertSettings(AlertSettingRequest[] request)
        {
            var resp = await _service.AlertSettingsAsync(request);
            if(!resp.Success)
                return StatusCode(500,resp);
            return Ok(resp);
        }

        [HttpGet("disaster-risks")]
        public async Task<IActionResult> GetRiskReport()
        {
            var resp = await _service.CreateRiskReportAsync();
            if(!resp.Success)
                return StatusCode(500,resp);
            return Ok(resp);
        }

        [HttpPost("alerts/send")]
        public async Task<IActionResult> SendAlert()
        {
            var resp = await _service.SendEmail();
            // if(!resp.Success)
            //     return StatusCode(500,resp);
            return Ok(resp);
        }
    }
}
