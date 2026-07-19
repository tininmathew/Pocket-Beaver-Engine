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
    internal virtual void Start(){}
    internal virtual void Update(float deltaTime){}
}