using LionFire.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Machine.Services
{


    public class ServiceControlService : IServiceControlService
    {
        public bool Start(string name)
        {
            return SupervisorCtl.Run($" start {name}");
        }
        public bool Stop(string name)
        {
            return SupervisorCtl.Run($" stop {name}");
        }
        public bool Restart(string name)
        {
            return SupervisorCtl.Run($" restart {name}");
        }
        public string Status(string name)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}
