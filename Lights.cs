using Assimp;
using Engine.Scripts;
using OpenTK.Mathematics;

namespace Engine;

public class PointLight : GameObject
{
    public Vector3 Color;
    public float Intensity;
    public PointLight(Vector3 pos, Vector3 color, float inten, string name, Scene scene, Mesh? mesh) : base(name, ObjParser.LoadMesh("models/quad.obj"), scene, position: pos)
    {
        Color = color;
        Intensity = inten;
        AddComponent(new OverrideTexture("resources/lamp.png", 0));
        AddComponent(new Rotater(new Vector3(0, 10, 0)));
    }
}
public class DirLight
{
    public static Vector3 Rotation;
    public static Vector3 Color;
    public static float Intensity;
}