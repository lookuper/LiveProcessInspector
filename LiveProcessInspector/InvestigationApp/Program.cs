using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvestigationApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string output = null;
			var a = new FullDump();
			//var file = a.CreateFullDump(6980, out output);			//var dir = AppDomain.CurrentDomain.BaseDirectory;
			//var name = "procdump.exe";
			//var res2 = Path.GetFullPath(name);		

			//var r = Process.Start(res2);
			//r.WaitForExit();
			//var res1 = Properties.Resources.procdump1;
			//var test = AppDomain.CreateDomain("test");

			//test.Load(res1);

			int i = 1;
			double d = 1.1;
			string s = "string s";
			TimeSpan ts = new TimeSpan();

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Task.Factory.StartNew(() =>
			{
				Thread.Sleep(TimeSpan.FromSeconds(30));
				i = 2;
			});

			//Task.Delay(TimeSpan.FromSeconds(30))
			//	.ContinueWith((res) =>
			//	{
			//		i = 3;
			//	});

			var newDomain = AppDomain.CreateDomain("newDomain");
			var anotherThread = new Thread(new ThreadStart(() =>
			{
				i = 11;
				d = 1.11;
				s = "string s1";
				ts = new TimeSpan();

				throw new ArgumentNullException();
			}));

			anotherThread.Start();

			//anotherThread.Abort(null);
			//Thread.CurrentThread.Abort();			
			//Console.ReadLine();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MiniDump.CreateMiniDump();
		}

		public static IEnumerable<String> GetSomeString()
		{
			return new[] { "1", "2" };
		}
	}

	public class Foo
	{
		private readonly int _someValue = 4;
		private const int ANOTHER_VALUE = 5;
		public int MyProperty { get; private set; }

		public Foo()
		{
			MyProperty = -0;
		}
	}
}
