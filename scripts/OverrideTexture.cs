namespace Engine;

public class OverrideTexture : Script
{
    string pathToTexture = "";
    string oldPath;
    Material? material;
    int matNum;
    public OverrideTexture(string path, int mat)
    {
        pathToTexture = path;
        oldPath = path;
        matNum = mat;
    }
    internal override void Start()
    {
        if(gameObject.Mesh == null) return;
        material = gameObject.Mesh.materials[matNum];
        material.Texture = TextureLoader.LoadTexture(pathToTexture);
    }
    internal override void Update(float deltaTime)
    {
        if(material == null) return;
        if(oldPath == pathToTexture) return;
        material.Texture = TextureLoader.LoadTexture(pathToTexture);
        oldPath = pathToTexture;
    }
}