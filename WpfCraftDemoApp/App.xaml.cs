using System.Windows;
using WpfCraftDemoApp.Services;
using WpfCraftDemoApp.ViewModels;

namespace WpfCraftDemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Properties.Settings settings = new Properties.Settings();
            IService service = new FlickrService(settings.FlickrServiceUrl);
            IViewModelHelper viewModelHelper = new ViewModelHelper();
            
            Window window = new MainWindow();
            window.DataContext = new ViewModels.ViewModel(service, window.Dispatcher, viewModelHelper);
            
            window.Show();
        }
    }
}
