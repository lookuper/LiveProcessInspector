﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LiveProcessInspector.Utils
{
	public class SelectedProcessUtil
	{
		[DllImport("user32.dll")]
		static extern IntPtr WindowFromPoint(Win32Point Point);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		[DllImport("user32.dll")]
		static extern bool SetWindowText(IntPtr hWnd, string lpString);

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool BringWindowToTop(IntPtr hWnd);

		private static Win32Point GetMousePosition()
		{
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);

			return w32Mouse;
		}

		public static int GetProcessAfterLeftMouseUp()
		{
			Win32Point clickPosition = GetMousePosition();
			IntPtr hWnd = WindowFromPoint(clickPosition);

			//var res = BringWindowToTop(hWnd);
			uint processID = uint.MinValue;
			var threadId = GetWindowThreadProcessId(hWnd, out processID);

			checked
			{
				return (int)processID;
			}
		}
	}
}
