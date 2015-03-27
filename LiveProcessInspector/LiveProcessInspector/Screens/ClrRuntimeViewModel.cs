using Caliburn.Micro;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveProcessInspector.Screens
{
	[Export(typeof(ClrRuntimeViewModel))]
	public class ClrRuntimeViewModel : PropertyChangedBase
	{
		private readonly ClrRuntime _runtime;
		private readonly ClrHeap _heap;

		public ClrRuntimeViewModel(ClrRuntime runtime)
		{
			_runtime = runtime;
			_heap = _runtime.GetHeap();

			//foreach (var obj in _heap.EnumerateObjects())
			//{
			//	clr
			//}

			//var t = (from obj in _heap.EnumerateObjects()
			//		 let type = _heap.GetObjectType(obj)
			//		 let size = type.GetSize(obj)
			//		 select new
			//		 {
			//			 Object = obj,
			//			 Size = size,
			//			 Name = type.Name
			//		 }).ToList();
			//var i = 5;
		}

		public bool IsServerGC { get { return _runtime.ServerGC; } }
		public int PointerSize { get { return _runtime.PointerSize; } }
		public int HeadCount { get { return _runtime.HeapCount; } }
		public IEnumerable<ClrAppDomain> AppDomains { get { return _runtime.AppDomains; } }
		public IList<ClrThread> Threads { get { return _runtime.Threads; } }
		public ClrThreadPool ThreadPool { get { return _runtime.GetThreadPool(); } }
		public bool CanWalkHelp { get { return _heap.CanWalkHeap; } }

		private ClrThread _selectedThread = null;
		public ClrThread SelectedThread
		{
			get { return _selectedThread; }
			set
			{
				_selectedThread = value;
				NotifyOfPropertyChange(() => SelectedThread);
			}
        }
	}
}
