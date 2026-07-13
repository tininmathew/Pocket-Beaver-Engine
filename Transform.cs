using OpenTK.Mathematics;

namespace Engine;

public class Transform
{
    public Vector3 Position;
    /// <summary>
    /// Radians rotation. For angles do the "SetAngles();"
    /// </summary>
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.One;
    public void SetAngles(Vector3 angles)
    {
        Rotation = angles * (float)(Math.PI/180d);
    }
    public Vector3 GetAngles()
    {
        return Rotation * (float)(180d/Math.PI);
    }

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