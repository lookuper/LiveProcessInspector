using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LiveProcessInspector.Utils
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Win32Point
	{
		public Int32 X;
		public Int32 Y;
	};
}
