using OpenTK.Mathematics;

namespace Engine;

public class Transform
{
    public Vector3 Position, Rotation;
    public Vector3 Scale = Vector3.One;

    public Matrix4 ModelMatrix
    {
        get
        {
            return
                Matrix4.CreateScale(Scale)
                *
                Matrix4.CreateRotationX(Rotation.X)
                *
                Matrix4.CreateRotationY(Rotation.Y)
                *
                Matrix4.CreateRotationZ(Rotation.Z)
                *
                Matrix4.CreateTranslation(Position);
        }
    }
}