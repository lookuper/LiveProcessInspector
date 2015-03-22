using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace LiveProcessInspector
{
    public class InspectorModel
    {
        public List<String> GetAvaliableProcesses()
        {
            var output = new ConcurrentStack<Process>();
            Parallel.ForEach(Process.GetProcesses(), (proc) =>
            {
                try
                {
                    Parallel.For(0, proc.Modules.Count, (i, loopState) =>
                    {
                        var moduleName = proc.Modules[i].ModuleName;
                        if (Desktop40LanguageRuntime().Contains(moduleName))// || OldDesktopLanguageRuntime().Contains(moduleName))
                        {
                            output.Push(proc);
                            loopState.Break();
                        }
                    });
                }
                catch (Win32Exception ex) // access is denied
                {}
            });

            return output
                .OrderBy(x => x.ProcessName)
                .Select(x => x.ProcessName)
                .ToList();
        }

        public bool TryToAttach(String selectedProcess, out DataTarget outTarget)
        {
            outTarget = null;
            var neededProc = Process.GetProcesses()
                .Where(x => x.ProcessName.Equals(selectedProcess, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (neededProc == null)
                return false;

            outTarget = DataTarget.AttachToProcess(neededProc.Id, 4000);
            return true;
        }

        public bool TryToOpenDump(String path, out DataTarget outTarget)
        {
            outTarget = null;

            if (!File.Exists(path))
                return false;

            outTarget = DataTarget.LoadCrashDump(path);
            return true;
        }








        private IEnumerable<String> Desktop40LanguageRuntime()
        {
            return new[] { "mscoree.dll", "mscoreei.dll" };
        }

        private IEnumerable<String> OldDesktopLanguageRuntime()
        {
            return new[] { "mscorwks.dll" };
        }
    }
}
