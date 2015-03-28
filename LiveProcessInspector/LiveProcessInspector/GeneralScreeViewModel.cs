using Caliburn.Micro;
using LiveProcessInspector.Screens;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LiveProcessInspector
{
    [Export(typeof(GeneralScreenViewModel))]
    public class GeneralScreenViewModel : Conductor<Object>
    {
        private readonly InspectorModel _model = new InspectorModel();
        private readonly IWindowManager _windowManager;
		private DataTarget _dataTarget;
		private ClrRuntime _clrRuntime;

        [ImportingConstructor]
        public GeneralScreenViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
			DisplayName = "Live Process Inspector v0.1 alpha";

			OpenClrRuntime();
        }

		public bool IsDataTargetAvaliable
		{
            get { return _dataTarget != null; }
		}

		public bool IsRuntimeAvalible
		{
			get { return _clrRuntime != null; }
		}

		public void OpenDataTarget()
		{
			if (_dataTarget == null)
				OpenDump();

			var viewModel = new DataTargetViewModel(_dataTarget);
			ActivateItem(viewModel);
		}

		public void OpenClrRuntime()
		{
			OpenDump(); // to speed up development
			if (_dataTarget == null)
				throw new ArgumentNullException("_dataTarget");

			var clrVersion = _dataTarget.ClrVersions.FirstOrDefault();
			if (clrVersion == null)
				throw new ArgumentNullException("clrVersion");

			if (_clrRuntime == null)
				_clrRuntime = _dataTarget.CreateRuntime(clrVersion.TryGetDacLocation() ?? clrVersion.TryDownloadDac());

			var viewModel = new ClrRuntimeViewModel(_clrRuntime);
			ActivateItem(viewModel);
		}

		protected override void OnDeactivate(bool close)
		{
			using (_dataTarget) { }
			base.OnDeactivate(close);
		}

		public void OpenDump()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Dump files (*.dmp)|*.dmp";
			dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			string path;
			//var res = dlg.ShowDialog();

			//if (res.HasValue && res.Value)
			//    path = dlg.FileName;
			//else
			//{
			//    //MessageBox.Show("Cannot find such file");
			//    return;
			//}

			path = @"C:\Users\Maksym.Chernenko\Desktop\LiveProcessInspectorMini.dmp";
			//path = @"C:\Users\Maksym.Chernenko\Desktop\LiveProcessInspectorMiniNotebook.dmp";

			if (_model.TryToOpenDump(path, out _dataTarget))
			{
				var location = _dataTarget.ClrVersions[0].TryGetDacLocation();
				var locationFromNet = _dataTarget.ClrVersions[0].TryDownloadDac();

				// refactor
				try
				{
					_clrRuntime = _dataTarget.CreateRuntime(location ?? locationFromNet);
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
                if (_model.TryToAttach(avaliableProcessModel.SelectedProcess, out _dataTarget))
                {
                    var location = _dataTarget.ClrVersions[0].TryGetDacLocation();
                    //var runtime = _dataTarget.CreateRuntime(location);
                }

            }
            else
                return;

        }
    }
}
