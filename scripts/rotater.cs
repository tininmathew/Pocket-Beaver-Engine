namespace Engine.Scripts;

public class Rotater : Script
{
    internal override void Start()
    {
        
    }
    internal override void Update(float deltaTime)
    {
        gameObject.Transform.Rotation.X += deltaTime;
        gameObject.Transform.Rotation.Y += deltaTime;
        gameObject.Transform.Rotation.Z += deltaTime;
    }
}