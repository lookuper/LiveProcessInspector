using Caliburn.Micro;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LiveProcessInspector
{
    [Export(typeof(GeneralScreenViewModel))]
    public class GeneralScreenViewModel : PropertyChangedBase
    {
        private readonly InspectorModel model = new InspectorModel();
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public GeneralScreenViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
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

            path = @"C:\Users\Maksym.Chernenko\Desktop\SnippetStoreMini.dmp";
            DataTarget target;

            if (model.TryToOpenDump(path, out target))
            {
				var location = target.ClrVersions[0].TryGetDacLocation();
				var locationFromNet = target.ClrVersions[0].TryDownloadDac();
				try
				{
                    var runtime = target.CreateRuntime(location);
				}
				catch (ClrDiagnosticsException ex)
				{
					var runtime = target.CreateRuntime(location);
				}
                //target.CreateRuntime(@"C:\Windows\Microsoft.NET\Framework64\v2.0.50727\mscordacwks.dll");
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
                DataTarget target;
                if (model.TryToAttach(avaliableProcessModel.SelectedProcess, out target))
                {
                    var location = target.ClrVersions[0].TryGetDacLocation();
                    var runtime = target.CreateRuntime(location);
                }

            }
            else
                return;

        }
    }
}
