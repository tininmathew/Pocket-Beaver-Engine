namespace Engine.Scripts;

public class Mover : Script
{
    internal override void Update(float deltaTime)
    {
        gameObject.Transform.Position.X += deltaTime;
    }
}