using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.PullAgent
{
    public class RepoOptions
    {
        public string Url { get; set; }
        public string SshUrl { get; set; }
        public string Path {
            get {
                if (Directory.Exists(@"C:\"))
                {
                    return WindowsPath;
                }
                return UnixPath;
            }
        }
        public string WindowsPath { get; set; }
        public string UnixPath { get; set; }
        public string Tag { get; set; }
        public string Branch { get; set; }
    }
}
