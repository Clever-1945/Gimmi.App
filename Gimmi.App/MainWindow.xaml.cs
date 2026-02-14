using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gimmi.App.Controls;
using Gimmi.App.SettingsApp;
using NHotkey.Wpf;

namespace Gimmi.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool isLoaded;

    private List<HelpSession> helpSessions = new List<HelpSession>();
    
    public MainWindow()
    {
        InitializeComponent();
        var appSettings = DbRepository.FindAll<AppSize>().FirstOrDefault();
        if (appSettings?.Width != null)
        {
            this.Width = appSettings.Width.Value;
        }
        if (appSettings?.Height != null)
        {
            this.Height = appSettings.Height.Value;
        }

        HotkeyManager.Current.AddOrReplace(
            "Gimmi.App",
            Key.F1,
            ModifierKeys.Control,
            (o, args) =>
            {
                ActiveHelp();
            });

        this.Closing += (sender, args) => OnClosing(args);
        this.SizeChanged += (sender, args) => OnSizeChanged(args);
        this.Loaded += (sender, args) => OnLoaded(args);
    }

    private void OnLoaded(RoutedEventArgs args)
    {
        isLoaded = true;
        ApplyTabs();
        ToCenter();
    }
    
    private void OnClosing(CancelEventArgs args)
    {
        this.Hide();
        args.Cancel = true;
    }

    private void OnSizeChanged(SizeChangedEventArgs args)
    {
        if (isLoaded)
        {
            SaveSize();
        }
    }

    private void SaveSize()
    {
        DbRepository.AddToStack(null, (db, data) =>
        {
            var appSettings = db.GetCollection<AppSize>().FindAll().FirstOrDefault();
            
            if (appSettings == null)
            {
                appSettings = new AppSize();
                appSettings.Width = (int)this.ActualWidth;
                appSettings.Height = (int)this.ActualHeight;
                appSettings.Insert(db);
            }
            else
            {
                appSettings.Width = (int)this.ActualWidth;
                appSettings.Height = (int)this.ActualHeight;
                appSettings.Update(db);
            }
        });
    }

    private void ActiveHelp()
    {
        this.Show();
        this.Activate();
        this.Focus();
        ToCenter();
        AddNewSession();
    }

    private void ToCenter()
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double windowWidth = this.Width;
        double windowHeight = this.Height;

        this.Left = (screenWidth / 2) - (windowWidth / 2);
        this.Top = (screenHeight / 2) - (windowHeight / 2);
    }

    private void AddNewSession()
    {
        var session = CreateHelpSession();
        helpSessions.Add(session);
        ApplyTabs();
        GoToHelpSession(session);
    }
    
    private void ApplyTabs()
    {
        var stackPanel = NamePanelTabs.Content as StackPanel;
        if (stackPanel.Children.Count < 1)
        {
            var addButton = new Button();
            stackPanel.Children.Add(addButton);
            addButton.Content = "+";
            addButton.Click += (sender, args) =>
            {
                AddNewSession();
            };
        }

        var buttons = new HashSet<Button>(helpSessions.Select(x => x.Button));
        var stackButtons = stackPanel.Children.Cast<Button>().Take(stackPanel.Children.Count - 1).ToArray();
        var hasStackButtons = new HashSet<Button>(stackButtons);
        foreach (var button in hasStackButtons.Where(x => !buttons.Contains(x)))
        {
            stackPanel.Children.Remove(button);
        }

        foreach (var button in buttons.Where(x => !hasStackButtons.Contains(x)))
        {
            stackPanel.Children.Insert(0, button);
        }
    }

    private HelpSession CreateHelpSession()
    {
        var session = new HelpSession();
        session.Button = new Button();
        session.Button.Content = "Google";
        session.Button.Tag = session;
        session.Button.Click += (sender, args) => GoToHelpSession((sender as Button)?.Tag as HelpSession);
        session.Button.MouseUp += (sender, args) =>
        {
            if (args.ChangedButton == MouseButton.Middle)
            {
                var session = (sender as Button)?.Tag as HelpSession;
                if (session != null)
                {
                    helpSessions = helpSessions.Where(x => x != session).ToList();
                    ApplyTabs();
                    session = helpSessions.FirstOrDefault(x => x.IsSelected) ?? helpSessions.FirstOrDefault();
                    if (session != null)
                    {
                        GoToHelpSession(session);
                    }
                }
            }
        };
        session.WebControl = new ControlWeb();
        session.WebControl.SetUrl(new Uri("https://www.google.com/search?q=&udm=50&sclient=gws-wiz"));
        session.WebControl.OnChangeTitle = (t) => session.ApplyButtonContent();

        return session;
    }

    private void GoToHelpSession(HelpSession session)
    {
        for (int i = NameGridMain.Children.Count - 1; i >= 0; i--)
        {
            var element = NameGridMain.Children[i];
            if (element is ControlWeb)
            {
                NameGridMain.Children.Remove(element);
            }
        }

        session.WebControl.SetValue(Grid.RowProperty, 0);
        session.WebControl.SetValue(Grid.ColumnProperty, 2);

        NameGridMain.Children.Add(session.WebControl);
        session.IsSelected = true;
        session.ApplyButtonContent();
        foreach (var helpSession in helpSessions.Where(x => x != session))
        {
            helpSession.IsSelected = false;
            helpSession.ApplyButtonContent();
        }
        
        session.WebControl.SetFocus();
    }
}