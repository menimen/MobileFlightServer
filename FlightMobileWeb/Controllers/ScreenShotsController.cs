using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Drawing;
using System;
using System.Text;

namespace FlightAppServer
{
    [Route("api/screenshot")]
    [ApiController]
    public class ScreenShotsController : ControllerBase
    {

        // GET: api/ScreenShots
        [HttpGet]
        public byte[] GetscreenShots()
        {
            return Data.image_bytes;
        }
    }
}
