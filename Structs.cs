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
    // треугольников * 3 в сабмеше
    public int IndexCount { get; set; }
    
    // с какого индекса в общем EBO начинается этот субмеш
    public int StartIndex { get; set; }
    public int MaterialId { get; set; } 
}
