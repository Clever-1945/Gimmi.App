using System.Windows.Controls;
using Gimmi.App.Controls;

namespace Gimmi.App;

public class HelpSession
{
    public Button Button { set; get; }
    
    public ControlWeb WebControl { set; get; }
    
    public bool IsSelected { set; get; }

    public void ApplyButtonContent()
    {
        WebControl.GetTitle((t) =>
        {
            Button.Dispatcher.Invoke(() =>
            {
                Button.Content = $"{(IsSelected ? "✔" : "")} {t}".Trim();
                Button.ToolTip = t;
            });
        });
    }
}