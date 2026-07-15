using OpenTK.Mathematics;

namespace Engine.Scripts;

public class Rotater : Script
{
    Vector3 direction;
    float speed;
    public Rotater(Vector3 dir, float _speed)
    {
        direction = Vector3.Normalize(dir);
        speed = _speed;
    }
    internal override void Start()
    {
        
    }
    internal override void Update(float deltaTime)
    {
        gameObject.Transform.Rotation *= Quaternion.FromAxisAngle(direction, deltaTime * speed);
    }
}