using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase where T : ControllerBase
    {
        protected readonly ILogger<T> _logger;

        protected BaseApiController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}