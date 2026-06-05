using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Engine;

internal class Program
{
    static void Main()
    {
        Console.WriteLine(Marshal.SizeOf<Vector3>());
        Console.WriteLine(Marshal.SizeOf<Vertex>());

        var gameSettings = GameWindowSettings.Default;

        var nativeSettings = new NativeWindowSettings()
        {
            Title = Constants.WindowName,
            ClientSize = new OpenTK.Mathematics.Vector2i((int)Constants.ScreenSize.X, (int)Constants.ScreenSize.Y),
            WindowState = WindowState.Maximized
        };

        using var window = new Game(gameSettings, nativeSettings);

        window.Run();
    }
}