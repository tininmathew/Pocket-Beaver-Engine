using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Engine;

public class Camera
{
    public Vector3 Position;

    public float Yaw = -90f;
    public float Pitch = 0f;

    public Vector3 Front { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; }
    public Matrix4 Projection = Matrix4.CreatePerspectiveFieldOfView
    (
        MathHelper.DegreesToRadians(70f),
        Constants.ScreenSize.X / Constants.ScreenSize.Y,
        0.1f,1000f
    );

    public Matrix4 GetViewMatrix()
    {
        Vector3 front;

        front.X =
            MathF.Cos(MathHelper.DegreesToRadians(Yaw))
            * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

        front.Y =
            MathF.Sin(MathHelper.DegreesToRadians(Pitch));

        front.Z =
            MathF.Sin(MathHelper.DegreesToRadians(Yaw))
            * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

        front = Vector3.Normalize(front);

        return Matrix4.LookAt(
            Position,
            Position + front,
            Vector3.UnitY);
    }

    internal void UpdateVectors()
    {
        Front = new Vector3
        (
            MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch)),

            MathF.Sin(MathHelper.DegreesToRadians(Pitch)),

            MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch))
        );

        Front = Vector3.Normalize(Front);

        Right = Vector3.Normalize(
            Vector3.Cross(
                Front,
                Vector3.UnitY));

        Up = Vector3.Normalize(
            Vector3.Cross(
                Right,
                Front));
    }
}