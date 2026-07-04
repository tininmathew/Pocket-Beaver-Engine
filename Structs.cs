using OpenTK.Mathematics;

namespace Engine;

public struct Vertex
{
    public Vector3 position;
    public Vector3 normal;
    //public Vector2 uv;
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
    public int ShaderProgramId { get; set; }
    public Vector3 Color { get; set; } // Основной цвет (RGB)

    public Material(int shaderProgramId, Vector3 color)
    {
        ShaderProgramId = shaderProgramId;
        Color = color;
    }
}