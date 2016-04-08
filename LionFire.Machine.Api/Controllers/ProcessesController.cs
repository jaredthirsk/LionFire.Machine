using LionFire.Machine.Processes.Linux;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Machine.Api.Controllers
{
    [Route("machine/[controller]")]
    public class ProcessesController : Controller
    {
        [HttpGet("[action]")]
        public LoadAvgFile LoadAvg()
        {
            return LoadAvgFile.Retrieve();
        }
    }
}
