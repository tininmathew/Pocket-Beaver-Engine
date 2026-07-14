using OpenTK.Mathematics;

namespace Engine;

public struct Vertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;
};
public struct Submesh
{
    // Сколько индексов (треугольников * 3) принадлежит этому субмешу
    public int IndexCount { get; set; }
    
    // С какого индекса в общем EBO начинается этот субмеш
    public int StartIndex { get; set; }
    
    // ID или ссылка на материал, которым это красить
    public int MaterialId { get; set; } 
}
public class Material
{
    public string Name;
    public Vector3 Diffuse { get; set; }
    public Vector3 Ambient { get; set; }
    public Vector3 Specular { get; set; }
    public float Transparency { get; set; }
    public int Texture { get; set; }
    public bool IsTransparent { get; set; }

    public Material(string name, Vector3? dif = null)
    {
        Name = name;
        Diffuse = dif ?? Vector3.One;
        Transparency = 1;
        Texture = TextureLoader.LoadTexture("/mnt/data/C#P/PocketBeaver/resources/fallback texture.bmp");
    }
}