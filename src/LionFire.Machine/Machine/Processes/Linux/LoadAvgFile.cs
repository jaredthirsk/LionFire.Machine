using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Machine.Processes.Linux
{
    public class LoadAvgFile
    {
        public float Last1 { get; set; }
        public float Last5 { get; set; }
        public float Last15 { get; set; }
        public int RunningProcessCount { get; set; }
        public int ProcessCount { get; set; }

        public static LoadAvgFile Retrieve()
        {
            string line;
            using (var sr = new StreamReader(new FileStream("/proc/loadavg", FileMode.Open, FileAccess.Read)))
            {
                line = sr.ReadLine();
            }
            var parts = line.Split(' ');
            var procs = parts[3].Split('/');

            return new LoadAvgFile()
            {
                Last1 = float.Parse(parts[0]),
                Last5 = float.Parse(parts[1]),
                Last15 = float.Parse(parts[2]),
                RunningProcessCount = Int32.Parse(procs[0]),
                ProcessCount = Int32.Parse(procs[1]),
            };
        }
    }
}
