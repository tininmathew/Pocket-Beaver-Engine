using Engine.Graphics;
using OpenTK.Mathematics;

namespace Engine;

public class GameObject
{
    Scene currentScene;
    public GameObject(string name, Mesh mesh, Scene scene, 
Vector3 position = default, Vector3 rotation = default, Vector3? scale = null,
List<Script>? comps = null)
    {
        Vector3 _scale = scale ?? Vector3.One;
        this.Name = name;
        this.Mesh = mesh;
        currentScene = scene;
        currentScene.Add(this);
        Transform.Position = position;
        Transform.Rotation = rotation;
        Transform.Scale = _scale;
        Components = comps??new List<Script>();
        foreach(Script i in Components)
        {
            i.Set(this, scene);
            i.Start();
        }
    }
    public string Name { get; set; }
    public Mesh Mesh { get; set; }
    public List<Script> Components = new List<Script>();
    public Transform Transform { get; } = new();
    public void Update(float time)
    {
        foreach(Script i in Components)
        {
            i.Update(time);
        }
    }
    public void Render(Shader shader)
    {
        shader.SetMatrix4("model",this.Transform.ModelMatrix);
        this.Mesh.Draw();
    }
    public void AddComponent(Script toadd)
    {
        Components.Add(toadd);
        toadd.Set(this, currentScene);
        toadd.Start();
    }
}