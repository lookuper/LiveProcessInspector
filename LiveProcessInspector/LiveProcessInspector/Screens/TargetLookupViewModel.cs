using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveProcessInspector.Screens
{
	[Export(typeof(TargetLookupViewModel))]
	public class TargetLookupViewModel : PropertyChangedBase
	{
		private readonly int _processPid = 0;
		private readonly string _processName;
		private readonly string _processTitle;

		public int ProcessPid { get { return _processPid; } }
		public string ProcessName { get { return _processName; } }
		public string ProcessTitle { get { return _processTitle; } }

		public TargetLookupViewModel(int processPid)
		{
			_processPid = processPid;

			var process = Process.GetProcessById(_processPid);
            _processName = process.ProcessName;
			_processTitle = process.MainWindowTitle;
		}
	}
}
