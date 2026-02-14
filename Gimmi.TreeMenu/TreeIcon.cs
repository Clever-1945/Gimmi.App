namespace Gimmi.TreeMenu;

public static class TreeIcon
{
    private static NotifyIcon _notifyIcon;
    
    public static void Create()
    {
        _notifyIcon = new NotifyIcon();
        _notifyIcon.Visible = true;
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
    }

    public static void SetText(string text)
    {
        _notifyIcon.Text = text;
    }
    
    public static void SetIcon(Icon icon)
    {
        _notifyIcon.Icon = icon;
    }
    
    public static void SetIcon(Stream icon)
    {
        _notifyIcon.Icon = new Icon(icon);
    }
    
    public static void Add(string text, Action action)
    {
        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem();
        exitMenuItem.Text = text;
        exitMenuItem.Click += (sender, args) => action?.Invoke();
        _notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
    }
    
    public static void Insert(int index, string text, Action action)
    {
        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem();
        exitMenuItem.Text = text;
        exitMenuItem.Click += (sender, args) => action?.Invoke();
        _notifyIcon.ContextMenuStrip.Items.Insert(index, exitMenuItem);
    }

    public static void Release()
    {
        _notifyIcon?.Dispose();
        _notifyIcon = null;
    }
}