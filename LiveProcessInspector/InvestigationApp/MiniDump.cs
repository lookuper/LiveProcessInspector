using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvestigationApp
{
	public static class MiniDump
	{
		[Flags]
		public enum Option : uint
		{
			// From dbghelp.h:
			Normal = 0x00000000,
			WithDataSegs = 0x00000001,
			WithFullMemory = 0x00000002,
			WithHandleData = 0x00000004,
			FilterMemory = 0x00000008,
			ScanMemory = 0x00000010,
			WithUnloadedModules = 0x00000020,
			WithIndirectlyReferencedMemory = 0x00000040,
			FilterModulePaths = 0x00000080,		
			WithProcessThreadData = 0x00000100,
			WithPrivateReadWriteMemory = 0x00000200,
			WithoutOptionalData = 0x00000400,
			WithFullMemoryInfo = 0x00000800,
			WithThreadInfo = 0x00001000,
			WithCodeSegs = 0x00002000,
			WithoutAuxiliaryState = 0x00004000,
			WithFullAuxiliaryState = 0x00008000,
			WithPrivateWriteCopyMemory = 0x00010000,
			IgnoreInaccessibleMemory = 0x00020000,
			ValidTypeFlags = 0x0003ffff,
		};

		[StructLayout(LayoutKind.Sequential, Pack = 4)]	 // Pack=4 is important! So it works also for x64!
		public struct MiniDumpExceptionInformation
		{
			public uint ThreadId;
			public IntPtr ExceptionPointers;
			[MarshalAs(UnmanagedType.Bool)]
			public bool ClientPointers;
		}

		[DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", 
			CallingConvention = CallingConvention.StdCall, 
			CharSet = CharSet.Unicode, 
			ExactSpelling = true,
			SetLastError = true)]
		static extern bool MiniDumpWriteDump(IntPtr hProcess,
			uint processId, 
			SafeHandle hFile, 
			uint dumpType, 
			ref MiniDumpExceptionInformation expParam,
			IntPtr userStreamParam, 
			IntPtr callbackParam);

		public static void CreateMiniDump(string fileName = null)
		{
			using (var currentProcess = Process.GetCurrentProcess())
			{
				if (String.IsNullOrEmpty(fileName))
					fileName = String.Format("Crash_Dump_{0}.dmp", currentProcess.ProcessName, DateTime.Now.ToShortDateString());

				var miniInfo = new MiniDumpExceptionInformation();
				miniInfo.ThreadId = (uint)Thread.CurrentThread.ManagedThreadId;
				miniInfo.ExceptionPointers = Marshal.GetExceptionPointers();
				miniInfo.ClientPointers = true;
				
                using (var fs = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), fileName), FileMode.Create))
				{
					MiniDumpWriteDump(currentProcess.Handle,
						(uint)currentProcess.Id,
						fs.SafeFileHandle,
						(uint)(Option.WithDataSegs | Option.WithFullMemory | Option.WithHandleData),
						ref miniInfo,
						IntPtr.Zero,
						IntPtr.Zero);
				}
            }
		}
	}
}
