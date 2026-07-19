using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;

namespace Engine;

public class Transform
{
    public Vector3 Position;
    public Vector3 WorldPosition;
    public Quaternion Rotation;
    public Vector3 Scale = Vector3.One;
    public Transform? Parent;
    public Vector3 Angles
    {
        get
        {
            return Rotation.ToEulerAngles() * (float)(180d / Math.PI); 
        }
        set
        {
            float radiansX = MathHelper.DegreesToRadians(value.X);
            float radiansY = MathHelper.DegreesToRadians(value.Y);
            float radiansZ = MathHelper.DegreesToRadians(value.Z);
            Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, radiansY) *
                    Quaternion.FromAxisAngle(Vector3.UnitX, radiansX) *
                    Quaternion.FromAxisAngle(Vector3.UnitZ, radiansZ);
        }
    }


    public Matrix4 ModelMatrix
    {
        get
        {
            Matrix4 matrix =
                Matrix4.CreateScale(Scale)
                *
                Matrix4.CreateFromQuaternion(Rotation)
                *
                Matrix4.CreateTranslation(Position);
            if(Parent != null)
            {
                matrix *= Parent.ModelMatrix;
            }
            WorldPosition = matrix.Row3.Xyz;
            return matrix;
        }
    }
}