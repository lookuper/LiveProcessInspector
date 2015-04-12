using Caliburn.Micro;
using InvestigationApp;
using Microsoft.Diagnostics.Runtime;
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
		private GeneralScreenViewModel generalScreenViewModel;

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

		public TargetLookupViewModel(int processPid, GeneralScreenViewModel generalScreenViewModel) : this(processPid)
		{
			this.generalScreenViewModel = generalScreenViewModel;
			this.generalScreenViewModel.CurrentDataTarget = null; 
			this.generalScreenViewModel.CurrentClrRuntime = null;
		}

		public void AttachToProcess()
		{
			// use inspector model here
			if (!_inspector.TryToAttach(ProcessPid, out generalScreenViewModel.CurrentDataTarget))
			{
				CanAttach = false;
				Message = "Cannot attach to service, try to create a dump";
			}
			else
				RefreshBarAndGoToDataTarget(generalScreenViewModel.CurrentDataTarget);
        }

		public void DumpProcess()
		{
			var dumpHelper = new FullDump();
			string output;

			var pathToDump = dumpHelper.CreateFullDump(ProcessPid, ProcessName, out output);
			DataTarget data;

			if (!_inspector.TryToOpenDump(pathToDump, out data))
			{
				Message = "Cannot create service dump: " + output;
			}

			RefreshBarAndGoToDataTarget(data);
		}

		private void RefreshBarAndGoToDataTarget(DataTarget data)
		{
			generalScreenViewModel.RefreshStatusBar();

			var viewModel = new DataTargetViewModel(data);
			generalScreenViewModel.CurrentDataTarget = data;
			generalScreenViewModel.ActivateItem(viewModel);
		}
	}
}
