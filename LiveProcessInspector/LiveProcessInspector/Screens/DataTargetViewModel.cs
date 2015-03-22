using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using System.IO;
using System.Diagnostics;

namespace LiveProcessInspector.Screens
{
	[Export(typeof(DataTargetViewModel))]
	public class DataTargetViewModel : PropertyChangedBase
	{
		private readonly DataTarget _dataTarget;


		public String Architecture
		{
			get { return _dataTarget.Architecture.ToString(); }
		}

		public bool IsMiniDump
		{
			get { return _dataTarget.IsMinidump; }
		}

		public uint PointerSize
		{
			get { return _dataTarget.PointerSize; }
		}

		public IEnumerable<ClrInfo> ClrVersion
		{
			get { return _dataTarget.ClrVersions; }
		}

		public IEnumerable<ModuleInfo> Modules
		{
            get { return _dataTarget.EnumerateModules(); }
		}

		public DataTargetViewModel(DataTarget _dataTarget)
		{
			this._dataTarget = _dataTarget;
		}

		public void OpenModuleLocation(ModuleInfo selectedModule)
		{
			if (selectedModule == null)
				return;

			if (File.Exists(selectedModule.FileName))
			{
				Process.Start("explorer.exe", "/select, " + selectedModule.FileName);
			}
		}

		public void OpenDacLocation(ClrInfo selectedClr)
		{
			if (selectedClr == null)
				return;

			var location = selectedClr.TryGetDacLocation() ?? selectedClr.TryDownloadDac();

			if (File.Exists(location))
				Process.Start("explorer.exe", "/select, " + location);
		}
    }
}
