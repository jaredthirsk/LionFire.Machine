using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Tools
{
    public class Sudo
    {
        public static IEnumerable<string> ExeSearchPath {
            get {
                yield return @"/usr/bin/sudo";
            }
        }

        public static string SudoExe {
            get {
                return ExeSearchPath.Where(p => File.Exists(p)).FirstOrDefault();
            }
        }

    }
}
