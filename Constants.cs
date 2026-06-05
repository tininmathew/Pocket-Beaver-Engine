using System.Drawing;
using System.Numerics;

namespace Engine;

static class Constants
{
    public static Vector2 ScreenSize {get;} = new Vector2(1920, 1080);
    public static float MouseSensibility = 0.25f;
    public static float Speed = 1;
    public static string WindowName = "Pocket-Beaver Engine";
    public static float[] bgColor =
    {
        0.3f,
        0.3f,
        0.3f,
        1.0f
    };
}