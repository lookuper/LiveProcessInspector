using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationApp
{
	public class FullDump
	{
		private string _procdump = "procdump.exe";
		public string CurrentDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		private string GetFullPathToProcDump()
		{
			if (!Directory.Exists(CurrentDirectory))
				throw new ArgumentException(nameof(CurrentDirectory));

			return Path.GetFullPath(_procdump);
		}

		public String CreateFullDump(int processPid, out string output)
		{
			var path = GetFullPathToProcDump();
			if (String.IsNullOrEmpty(path))
				throw new ArgumentException("Cannot find procdump.exe");

			var fileName = String.Format("{0}_{1}.dmp", AppDomain.CurrentDomain.FriendlyName, DateTime.Now.ToString("hms"));
            var process = new Process();
			process.StartInfo.FileName = _procdump;
			process.StartInfo.Arguments = String.Format("-ma {0} {1}", processPid, fileName);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;

			process.Start();
			output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			if (!File.Exists(Path.GetFullPath(fileName)))
				throw new InvalidOperationException("Cannot create dump file");

			return Path.GetFullPath(fileName);			
		}
	}
}
