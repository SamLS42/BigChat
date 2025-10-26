namespace BigChat.AppCore.Settings;

public class WindowState
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
    public bool IsMaximized { get; set; }
}
