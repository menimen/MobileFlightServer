using Microsoft.AspNetCore.Mvc;
using System;

namespace FlightAppServer.Controllers
{
    [Route("api/command")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        [HttpPost]
        public void PostCommand(Command command)
        {
            Data.aileron = command.aileron;
            Data.throttle = command.throttle;
            Data.elevator = command.elevator;
            Data.rudder = command.rudder;
        }
    }
}
