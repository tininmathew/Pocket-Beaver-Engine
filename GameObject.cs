using Engine.Graphics;
using OpenTK.Mathematics;

namespace Engine;

public class GameObject
{
    public GameObject(string name, Mesh mesh, Scene scene, 
        Vector3 position = default, Vector3 rotation = default, Vector3? _scale = null)
    {
        Vector3 scale = _scale ?? Vector3.One;
        this.Name = name;
        this.Mesh = mesh;
        scene.Add(this);
        Transform.Position = position;
        Transform.Rotation = rotation;
        Transform.Scale = scale;
    }
    public string Name { get; set; }
    public Mesh Mesh { get; set; }
    public Transform Transform { get; } = new();
    public void Render(Shader shader)
    {
        shader.SetMatrix4("model",this.Transform.ModelMatrix);
        this.Mesh.Draw();
    }
}