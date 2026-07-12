namespace Engine;

public abstract class Script
{
    internal void Set(GameObject go, Scene sc)
    {
        gameObject = go;
        scene = sc;
        Console.WriteLine(go.Name);
    }
    protected GameObject gameObject;
    protected Scene scene;
    internal abstract void Start();
    internal abstract void Update(float deltaTime);
}