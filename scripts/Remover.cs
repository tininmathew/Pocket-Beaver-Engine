namespace Engine.Scripts;

public class Remover : Script
{
    float timer;
    public Remover(float time)
    {
        timer = time;
    }
    internal override void Update(float deltaTime)
    {
        timer -= deltaTime;
        if(timer <= 0)
        {
            scene.Destroy(gameObject);
        }
    }
}