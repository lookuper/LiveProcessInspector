using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiveProcessInspector
{
    /// <summary>
    /// Interaction logic for GeneralView.xaml
    /// </summary>
    public partial class GeneralScreenView : UserControl
    {
		private Window window;
		public GeneralScreenView()
        {
            InitializeComponent();
		}

		private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			window = Window.GetWindow(this);

			//Task.Delay(TimeSpan.FromMilliseconds(100))
				
			Delay(100).ContinueWith(res =>
			{
				window.WindowState = WindowState.Normal;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			window = Window.GetWindow(this);
			window.WindowState = WindowState.Minimized;
		}

		private static Task Delay(int milliseconds)		// Asynchronous NON-BLOCKING method
		{
			var tcs = new TaskCompletionSource<object>();
			new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
			return tcs.Task;
		}
	}
}
