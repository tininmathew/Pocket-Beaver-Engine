using System.Diagnostics;
using OpenTK.Mathematics;

namespace Engine;

public class GameObject : IDisposable
{
    Scene currentScene;
    public GameObject(string name, Mesh? mesh, Scene scene, 
Vector3 position = default, Vector3 rotation = default, Vector3? scale = null,
List<Script>? comps = null, Transform? parent = null)
    {
        Vector3 _scale = scale ?? Vector3.One;
        this.Name = name;
        this.Mesh = mesh;
        currentScene = scene;
        currentScene.Add(this);
        Transform.Position = position;
        Transform.Angles = rotation;
        Transform.Scale = _scale;
        Transform.Parent = parent;
        Components = comps??new List<Script>();
        foreach(Script i in Components)
        {
            i.Set(this, currentScene);
            i.Start();
        }
        if(Mesh == null) return;
    }
    public string Name { get; set; }
    public Mesh? Mesh { get; set; }
    public List<Script>? Components = new List<Script>();
    public Transform Transform { get; } = new();
    public void Update(float time)
    {
        if(Components == null) return;
        foreach(Script i in Components)
        {
            i.Update(time);
        }
    }
    public void Render(Shader shader)
    {
        if(this.Mesh == null) return;
        this.Mesh.Draw(this.Transform.ModelMatrix, Name);
    }
    public void AddComponent(Script toadd)
    {
        if(Components == null)
            Components = new List<Script>();
        Components.Add(toadd);
        toadd.Set(this, currentScene);
        toadd.Start();
    }
    public void Destroy()
    {
        currentScene.Destroy(this);
    }
    public void Dispose()
    {
        if(this.Mesh != null) this.Mesh.Dispose();
    }
}