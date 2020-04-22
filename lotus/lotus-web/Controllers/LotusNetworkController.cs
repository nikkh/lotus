using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace lotus_web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotusNetworkController : ControllerBase
    {
       

        private readonly ILogger<LotusNetworkController> _logger;

        public LotusNetworkController(ILogger<LotusNetworkController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public LotusNetwork Get()
        {
            return new LotusNetwork();
        }
    }
}
