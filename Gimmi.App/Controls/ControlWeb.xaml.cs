using System.IO;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Gimmi.App.Controls;

public partial class ControlWeb : UserControl
{
    private TaskCompletionSource<bool> initialized = new TaskCompletionSource<bool>();
    
    public Action<string> OnChangeTitle { set; get; }
    
    public ControlWeb()
    {
        InitializeComponent();

        string userDataFolder = Path.Combine(Assistant.ApplicationPath, "web-data");
        var environmentTask = CoreWebView2Environment.CreateAsync(null, userDataFolder);
        environmentTask.GetAwaiter().OnCompleted(() =>
        {
            webView.EnsureCoreWebView2Async(environmentTask.Result);
            webView.CoreWebView2InitializationCompleted += (sender, args) =>
            {
                initialized.TrySetResult(args.IsSuccess);
                webView.CoreWebView2.DocumentTitleChanged += (o, o1) =>
                {
                    OnChangeTitle?.Invoke(webView.CoreWebView2.DocumentTitle);
                };
                webView.CoreWebView2.DOMContentLoaded += (o, eventArgs) =>
                {
                    webView.Focus();
                };
            };
        });
    }

    public void GetTitle(Action<string> action)
    {
        OnCompleted((web) =>
        {
            action?.Invoke(webView.CoreWebView2.DocumentTitle);
        });
    }
    
    public void SetFocus()
    {
        OnCompleted((web) =>
        {
            webView.Focus();
        });
    }

    public void OnCompleted(Action<WebView2> action)
    {
        initialized.Task.GetAwaiter().OnCompleted(() =>
        {
            Dispatcher.Invoke(() =>
            {
                action?.Invoke(webView);
            });
        });
    }

    public void SetUrl(Uri url)
    {
        this.OnCompleted((w) =>
        {
            w.Source = url;
        });
    }
}