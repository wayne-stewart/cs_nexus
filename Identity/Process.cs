using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;

namespace Identity
{
    public sealed class Process
    {
        public static bool RunningAsAdmin()
        {
            var principle = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principle.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void Elevate()
        {
            var proc_info = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = System.Reflection.Assembly.GetExecutingAssembly().CodeBase,
                Verb = "runas"
            };
            System.Diagnostics.Process.Start(proc_info);
        }
    }
}
