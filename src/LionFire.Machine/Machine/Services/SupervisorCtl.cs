using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Tools
{
    public class SupervisorCtl
    {
        public static IEnumerable<string> SupervisorCtlExeSearchPath {
            get {
                yield return @"/usr/bin/supervisorctl";
            }
        }

        public static string ExePath {
            get {
                return SupervisorCtlExeSearchPath.Where(p => File.Exists(p)).FirstOrDefault();
            }
        }

        public static bool Run(string args)
        {
            var psi = new ProcessStartInfo(ExePath, args);
            Console.WriteLine($"Running {typeof(SupervisorCtl).Name.ToLower()} with args: " + args);
            var p = Process.Start(psi);
            p.WaitForExit();
            Console.WriteLine($"{typeof(SupervisorCtl).Name.ToLower()} exited with code " + p.ExitCode);
            return p.ExitCode == 0;
        }
        
    }
}
