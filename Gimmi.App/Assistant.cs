using System.IO;

namespace Gimmi.App;

public static class Assistant
{
    public static string ApplicationFileName { get; } = typeof(Assistant).Assembly.Location;
    public static string ApplicationPath { get; } = Path.GetDirectoryName(ApplicationFileName);
}