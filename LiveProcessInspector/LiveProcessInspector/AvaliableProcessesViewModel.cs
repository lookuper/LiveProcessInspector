using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveProcessInspector
{
    [Export(typeof(AvaliableProcessesViewModel))]
    public class AvaliableProcessesViewModel : Conductor<Object>
    {
        private readonly InspectorModel model = new InspectorModel();
        private String _selectedProcess;
        private readonly IWindowManager _windowManager;

        public BindableCollection<String> Processes
        {
            get {return new BindableCollection<String>(model.GetAvaliableProcesses());}                    
        }
        
        public String SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                NotifyOfPropertyChange(() => SelectedProcess);
            }
        }

        public GeneralScreenViewModel GeneralModel { get; private set; }


        [ImportingConstructor]
        public AvaliableProcessesViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void AttachToProcess()
        {
            //_windowManager.ShowWindow(new GeneralViewModel(_windowManager), null, null);
            CanClose(b =>
            {
                
                TryClose(true);
            });
            //ActivateItem(new GeneralViewModel(_windowManager));

            //_windowManager.ShowWindow(new GeneralScreeViewModel(_windowManager), null, null);
        }  
    }
}
