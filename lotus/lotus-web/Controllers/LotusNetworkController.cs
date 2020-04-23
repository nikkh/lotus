using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lotus_web.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace lotus_web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotusNetworkController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CosmosContext _cosmosContext;
        private readonly ILogger<LotusNetworkController> _logger;

        public LotusNetworkController(IConfiguration config, CosmosContext cosmosContext, ILogger<LotusNetworkController> logger)
        {
            _logger = logger;
            _cosmosContext = cosmosContext;
            _config = config;
        }

        [HttpGet]

        public async Task<ActionResult<LotusNetwork>> Get()
        {
            LotusNetwork lotusNetwork = new LotusNetwork(_cosmosContext);
            await lotusNetwork.FillFromCosmos();
            return lotusNetwork;
        }
    }
}
