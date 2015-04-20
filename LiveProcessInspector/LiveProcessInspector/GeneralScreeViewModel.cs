using Caliburn.Micro;
using LiveProcessInspector.About;
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
using System.Windows.Controls;
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
		private IEnumerable<string> _avaliableDumps;

		[ImportingConstructor]
        public GeneralScreenViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
			DisplayName = "Live Process Inspector v1.0";

			_originalCursor = Mouse.OverrideCursor;
			_targetCursor = new Cursor(Application.GetResourceStream(new Uri("Icons/target.ico", UriKind.Relative)).Stream);
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
			var dumpNames = _inspector.GetDumpNames();

			DumpsSize = _inspector.GetAllDumpSize();
			AvaliableDumps = dumpNames;
			DumpList = String.Join(Environment.NewLine, dumpNames);
			IsClearButtonEnabled = !String.IsNullOrEmpty(DumpList);
        }

		public void MenuItemClick(RoutedEventArgs e)
		{
			var mItem = e?.OriginalSource as MenuItem;
			var dumpPath = mItem?.Header as String;

			OpenDump(dumpPath);
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

		public IEnumerable<string> AvaliableDumps
		{
			get { return _avaliableDumps; }
			set { _avaliableDumps = value; NotifyOfPropertyChange(() => AvaliableDumps); } 
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

			if (CurrentDataTarget != null)
			{
				var viewModel = new DataTargetViewModel(CurrentDataTarget);
				ActivateItem(viewModel);
			}
		}

		public void OpenClrRuntime()
		{
			if (CurrentDataTarget == null)
				OpenDump();

			if (CurrentDataTarget == null)
			{
				//throw new ArgumentNullException("_dataTarget");
				return;
			}

			try
			{
				var info = CurrentDataTarget.ClrVersions.Single();
			}
			catch (NullReferenceException ex)
			{
				// set error header
				MessageBox.Show(ex.Message, "Cannot opern CLR window", MessageBoxButton.OK, MessageBoxImage.Error);

				var viewModel2 = new DataTargetViewModel(CurrentDataTarget);
				ActivateItem(viewModel2);
				return;
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show("Cannot find CLR in this process", "Cannot open CLR window", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (SystemException ex)
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
			CurrentDataTarget = null;
			CurrentClrRuntime = null;

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Dump files (*.dmp)|*.dmp";
			dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			string path;
			var res = dlg.ShowDialog();

			if (res.HasValue && res.Value)
				path = dlg.FileName;
			else
			{
				return;
			}
			
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

		private void OpenDump(string path)
		{
			if (String.IsNullOrEmpty(path))
				throw new ArgumentException(nameof(path));

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

		public void About()
		{			
			AboutBoxSimple aboutBox = new AboutBoxSimple(Application.Current.MainWindow);
			aboutBox.ShowDialog();
		}
    }
}
