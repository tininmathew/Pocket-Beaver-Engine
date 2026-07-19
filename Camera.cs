using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Engine;

public class Camera
{
    public Vector3 Position;
    public Quaternion Rotation = Quaternion.Identity; 
    public Transform transform 
    {
        get
        {
            Transform ret = new Transform();
            ret.Position = Position;
            ret.Rotation = Rotation;
            return ret;
        }
        set
        {
            Position = value.WorldPosition;
            Rotation = value.Rotation;
        }
    }

    public Vector3 Front { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; }
    
    public Matrix4 Projection = Matrix4.CreatePerspectiveFieldOfView
    (
        MathHelper.DegreesToRadians(70f),
        Constants.ScreenSize.X / Constants.ScreenSize.Y,
        0.1f, 1000f
    );
    public Matrix4 OrthoProjection = Matrix4.CreateOrthographic
    (
        Constants.ScreenSize.X, Constants.ScreenSize.Y, 0.1f, 10f
    );

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.CreateTranslation(-Position) * Matrix4.CreateFromQuaternion(Rotation.Inverted());
    }

    internal void UpdateVectors()
    {
        Front = Vector3.Normalize(Vector3.Transform(-Vector3.UnitZ, Rotation));

        Right = Vector3.Normalize(Vector3.Transform(Vector3.UnitX, Rotation));

        Up = Vector3.Normalize(Vector3.Transform(Vector3.UnitY, Rotation));
    }
}
