using ns.Base.Log;
using ns.Communication;
using ns.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
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

namespace Nidhogg.Service {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private CommunicationManager _communicationManager;
        private SemaphoreSlim _serviceStopSignal = new SemaphoreSlim(0, 1);
        //private Task _serviceTask;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (!CoreSystem.Initialize()) {
                throw new Exception(string.Format("{0} could not be initialized!", nameof(CoreSystem)));
            }
            _communicationManager = new CommunicationManager();
            _communicationManager.Initialize();
            DataContext = _communicationManager;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _communicationManager.Finalize();
        }
    }

    [ServiceContract(SessionMode = SessionMode.Allowed, CallbackContract = typeof(IWellcomeDuplexCallback))]
    public interface IHelloWorldService {

        [OperationContract(IsOneWay = true)]
        void SayHello(string name);
    }

    public class HelloWorldService : IHelloWorldService {
        private IWellcomeDuplexCallback _welcomeCallback;

        public HelloWorldService() {
            _welcomeCallback = OperationContext.Current.GetCallbackChannel<IWellcomeDuplexCallback>();
        }

        public void SayHello(string name) {
            name = string.Format("Wellcome {0}!", name);
            _welcomeCallback.SayWellcome(name);
        }
    }

    public interface IWellcomeDuplexCallback {

        [OperationContract(IsOneWay = true)]
        void SayWellcome(string name);
    }
}