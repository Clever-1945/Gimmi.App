using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using Gimmi.TreeMenu;

namespace Gimmi.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    DispatcherTimer timer = new DispatcherTimer();
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        TreeIcon.Create();
        
        var iconUri = new Uri("pack://application:,,,/Images/google.ico", UriKind.Absolute);
        var info = Application.GetResourceStream(iconUri);
        TreeIcon.SetIcon(info.Stream);
        TreeIcon.SetText("Google Gimmi");
        
        TreeIcon.Add("Выход", () => {
            Application.Current.Shutdown();
        });

        timer.Interval = TimeSpan.FromSeconds(2);
        timer.Tick += (sender, args) => DbRepository.Flush();
        timer.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        timer.Stop();
        TreeIcon.Release();
        base.OnExit(e);
    }
}