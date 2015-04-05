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
		private readonly InspectorModel _inspector;
		private bool _canAttach = true;
		private string _message;

		public int ProcessPid { get { return _processPid; } }
		public string ProcessName { get { return _processName; } }
		public string ProcessTitle { get { return _processTitle; } }
		public bool CanAttach
		{
			get { return _canAttach; }
			set { _canAttach = value; NotifyOfPropertyChange(() => CanAttach); }
		}
		public string Message
		{
			get { return _message; }
			set { _message = value; NotifyOfPropertyChange(() => Message); }
		}

		public TargetLookupViewModel(int processPid)
		{
			_processPid = processPid;

			var process = Process.GetProcessById(_processPid);
            _processName = process.ProcessName;
			_processTitle = process.MainWindowTitle;
			_inspector = new InspectorModel();
		}

		public void AttachToProcess()
		{
			// use inspector model here
			if (!_inspector.CanAttach(_processPid))
			{
				CanAttach = false;
				Message = "Cannot attach to service, try to create a dump";
			}
		}

		public void DumpProcess()
		{
		}
	}
}
