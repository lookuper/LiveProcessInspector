using Caliburn.Micro;
using LiveProcessInspector.Screens;
using LiveProcessInspector.Utils;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;

namespace LiveProcessInspector
{
    [Export(typeof(GeneralScreenViewModel))]
    public class GeneralScreenViewModel : Conductor<Object>
    {
        private readonly InspectorModel _model = new InspectorModel();
        private readonly IWindowManager _windowManager;
		private double _dumpsSize = 555.43;
		private bool _isClearButtonEnabled = true;
		public string _dumpList = String.Empty;
        private InspectorModel _inspector = new InspectorModel();
		private Cursor _originalCursor;
		private Cursor _targetCursor;

		public DataTarget CurrentDataTarget;
		public ClrRuntime CurrentClrRuntime;

        [ImportingConstructor]
        public GeneralScreenViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
			DisplayName = "Live Process Inspector v0.1 alpha";

			_originalCursor = Mouse.OverrideCursor;
			_targetCursor = new Cursor(Application.GetResourceStream(new Uri("Icons/target.ico", UriKind.Relative)).Stream);
			//OpenClrRuntime();
			RefreshStatusBar();
        }

		public void MouseDown()
		{
			Mouse.OverrideCursor = _targetCursor;
		}

		public void SelectProcess()
		{			
			var res = SelectedProcessUtil.GetProcessAfterLeftMouseUp();
			var viewModel = new TargetLookupViewModel(res, this);
			ActivateItem(viewModel);

			Mouse.OverrideCursor = _originalCursor;
		}

		public void RefreshStatusBar()
		{
			DumpsSize = _inspector.GetAllDumpSize();
			DumpList = String.Join(Environment.NewLine, _inspector.GetDumpNames());
		}

		public bool IsClearButtonEnabled
		{
            get { return _isClearButtonEnabled; }
			set { _isClearButtonEnabled = value; NotifyOfPropertyChange(() => IsClearButtonEnabled); }
		}

		public string DumpList
		{
            get { return _dumpList; }
			set { _dumpList = value; NotifyOfPropertyChange(() => DumpList); }
		}

		public void DeleteAllDumps()
		{
			_inspector.DeleteAllDumps();
			RefreshStatusBar();
		}

		public double DumpsSize
		{
			get { return _inspector.GetAllDumpSize() ; }
			set { _dumpsSize = value; NotifyOfPropertyChange(() => DumpsSize); }
		}

		public bool IsDataTargetAvaliable
		{
            get { return CurrentDataTarget != null; }
		}

		public bool IsRuntimeAvalible
		{
			get { return CurrentClrRuntime != null; }
		}

		public override void CanClose(Action<bool> callback)
		{
			base.CanClose(callback);
		}

		public void OpenDataTarget()
		{
			if (CurrentDataTarget == null)
				OpenDump();

			var viewModel = new DataTargetViewModel(CurrentDataTarget);
			ActivateItem(viewModel);
		}

		public void OpenClrRuntime()
		{
			//OpenDump(); // to speed up development
			if (CurrentDataTarget == null)
				throw new ArgumentNullException("_dataTarget");

			try
			{
				var info = CurrentDataTarget.ClrVersions.Single();
			}
			catch (NullReferenceException ex)
			{
				// set error header
				var viewModel2 = new DataTargetViewModel(CurrentDataTarget);
				ActivateItem(viewModel2);
				return;
			}

			var clrVersion = CurrentDataTarget.ClrVersions.FirstOrDefault();
			if (clrVersion == null)
				throw new ArgumentNullException("clrVersion");

			try
			{
				if (CurrentClrRuntime == null)
					CurrentClrRuntime = CurrentDataTarget.CreateRuntime(clrVersion.TryGetDacLocation() ?? clrVersion.TryDownloadDac());
			}
			catch (ClrDiagnosticsException ex)
			{
				MessageBox.Show("Cannot open CLR runtime, due the: " + ex.Message);
				return;
			}

			var viewModel = new ClrRuntimeViewModel(CurrentClrRuntime);
			ActivateItem(viewModel);
		}

		protected override void OnDeactivate(bool close)
		{
			using (CurrentDataTarget) { }
			base.OnDeactivate(close);
		}

		public void OpenDump()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Dump files (*.dmp)|*.dmp";
			dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			string path;
			var res = dlg.ShowDialog();

			if (res.HasValue && res.Value)
				path = dlg.FileName;
			else
			{
				MessageBox.Show("Cannot find such file");
				return;
			}

			//path = @"C:\Procdump\LiveProcessInspector.vshost.exe_150401_230544.dmp";

			if (_model.TryToOpenDump(path, out CurrentDataTarget))
			{
				try
				{
					var viewModel2 = new DataTargetViewModel(CurrentDataTarget);
					ActivateItem(viewModel2);
				}
				catch (NullReferenceException ex)
				{
					// dump without clr process
				}
				catch (ClrDiagnosticsException ex)
				{ }
			}
		}


        public void AttachToProcess()
        {
            var avaliableProcessModel = new AvaliableProcessesViewModel(_windowManager);
            dynamic settings = new ExpandoObject();
            settings.WindowStyle = WindowStyle.ToolWindow;
            settings.ShowInTaskbar = false;
            settings.Title = "Test";

            bool? res = _windowManager.ShowDialog(avaliableProcessModel, null, settings);

            if (res.HasValue && res.Value)
            {
                if (_model.TryToAttach(avaliableProcessModel.SelectedProcess, out CurrentDataTarget))
                {
                    var location = CurrentDataTarget.ClrVersions[0].TryGetDacLocation();
                    //var runtime = _dataTarget.CreateRuntime(location);
                }

            }
            else
                return;

        }
    }
}
