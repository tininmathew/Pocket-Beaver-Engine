namespace Engine.Scripts;

public class Rotater : Script
{
    bool X,Y,Z;
    public Rotater(string dir)
    {
        if(dir.Length > 3) return;
        for(int i = 0; i < 2; i++)
        {
            switch(dir.ToLower()[i])
            {
                case 'x':
                    X=true;
                    break;
                case 'y':
                    Y=true;
                    break;
                case 'z':
                    Z=true;
                    break;
            }
        }
    }
    internal override void Start()
    {
        
    }
    internal override void Update(float deltaTime)
    {
        gameObject.Transform.Rotation.X += deltaTime * (X?1:0);
        gameObject.Transform.Rotation.Y += deltaTime * (Y?1:0);
        gameObject.Transform.Rotation.Z += deltaTime * (Z?1:0);
    }
}