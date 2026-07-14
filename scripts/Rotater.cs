using OpenTK.Mathematics;

namespace Engine.Scripts;

public class Rotater : Script
{
    Vector3 direction;
    public Rotater(Vector3 dir)
    {
        direction = dir;
    }
    internal override void Start()
    {
        
    }
    internal override void Update(float deltaTime)
    {
        gameObject.Transform.Rotation += deltaTime * direction;
    }
}