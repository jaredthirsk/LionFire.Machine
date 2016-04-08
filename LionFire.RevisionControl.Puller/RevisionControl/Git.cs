using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.RevisionControl
{
    public class Git
    {
        public static IEnumerable<string> GitExeSearchPath {
            get {
                yield return @"C:\Program Files (x86)\Git\bin\git.exe";
                yield return @"C:\Program Files\Git\bin\git.exe";
                yield return @"/usr/bin/git";
            }
        }

        public static string GitExe {
            get {

                return GitExeSearchPath.Where(p => File.Exists(p)).FirstOrDefault();

                //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                //{
                //    return 
                //}
                //else {
                //    return "/usr/bin/git";
                //}
            }
        }

        public static bool Clone(string uri, string workingDirectory = null, string fileName = "")
        {
            if (workingDirectory != null)
            {
                if (!Directory.Exists(workingDirectory))
                {
                    Directory.CreateDirectory(workingDirectory);
                }
            }

            var psi = new ProcessStartInfo(GitExe, " clone " + uri + " " + fileName);
            psi.WorkingDirectory = workingDirectory;
            Console.WriteLine("Starting git clone " + uri);
            var p = Process.Start(psi);
            p.WaitForExit();
            Console.WriteLine("git exited with code " + p.ExitCode);
            return p.ExitCode == 0;
        }

        public static bool Pull(string workingDirectory = null, string uri = null)
        {
            if (workingDirectory != null)
            {
                if (!Directory.Exists(workingDirectory))
                {
                    if (uri != null) { return Clone(uri, Path.GetDirectoryName(workingDirectory), Path.GetFileName(workingDirectory)); }
                    else { throw new Exception("Directory doesn't exist and no uri provided.  Can't pull."); }
                }
            }
            var psi = new ProcessStartInfo(GitExe, " pull");
            psi.WorkingDirectory = workingDirectory;
            Console.WriteLine("Starting git pull in " + workingDirectory);
            var p = Process.Start(psi);
            p.WaitForExit();
            Console.WriteLine("git exited with code " + p.ExitCode);
            return p.ExitCode == 0;
        }

        public static bool CheckoutTag(string workingDirectory , string tag)
        {
            if (workingDirectory == null)
            {
                throw new ArgumentException("workingDirectory does not exist");
            }
            var psi = new ProcessStartInfo(GitExe, " checkout tag/" + tag);
            psi.WorkingDirectory = workingDirectory;
            Console.WriteLine("Starting checkout tag: " + tag);
            var p = Process.Start(psi);
            p.WaitForExit();
            Console.WriteLine("git exited with code " + p.ExitCode);
            return p.ExitCode == 0;
        }
        public static bool CheckoutBranch(string workingDirectory, string branch)
        {
            if (workingDirectory == null)
            {
                throw new ArgumentException("workingDirectory does not exist");
            }
            var psi = new ProcessStartInfo(GitExe, " checkout " + branch);
            psi.WorkingDirectory = workingDirectory;
            Console.WriteLine("Starting checkout branch: " + branch);
            var p = Process.Start(psi);
            p.WaitForExit();
            Console.WriteLine("git exited with code " + p.ExitCode);
            return p.ExitCode == 0;
        }
    }

}
