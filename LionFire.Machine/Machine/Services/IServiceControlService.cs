using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Machine.Services
{
    public interface IServiceControlService
    {
        bool Start(string name);
        bool Restart(string name);
        bool Stop(string name);
        string Status(string name);
    }
}
