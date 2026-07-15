using Assimp;
using Engine.Scripts;
using OpenTK.Mathematics;

namespace Engine;

public class PointLight : GameObject
{
    public Vector3 Color;
    public float Intensity;
    public PointLight(Vector3 pos, Vector3 color, float inten, string name, Scene scene, Mesh? mesh) 
    : base(name, ObjParser.LoadMesh("resources/light.png", MeshType.Bilboard), scene, position: pos)
    {
        Color = color;
        Intensity = inten;
    }
}
public class DirLight
{
    public static Vector3 Rotation;
    public static Vector3 Color;
    public static float Intensity;
}