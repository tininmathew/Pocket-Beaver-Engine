using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using OpenTK.Windowing.Common.Input;
using StbImageSharp;

namespace Engine;

internal class Program
{
    static void Main()
    {
        var gameSettings = GameWindowSettings.Default;
        var nativeSettings = new NativeWindowSettings()
        {
            Title = Constants.WindowName,
            ClientSize = new Vector2i((int)Constants.ScreenSize.X, (int)Constants.ScreenSize.Y),
            WindowState = WindowState.Fullscreen, 
            NumberOfSamples = 4 // MSAA 4 
        };

        using var window = new Game(gameSettings, nativeSettings);

        window.Run();
    }
}