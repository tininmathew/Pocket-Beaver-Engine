using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Engine;

public class Mesh
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;
    
    // Список всех подмешей
    public readonly Submesh[] submeshes;
    public readonly Material[] materials;


    public Mesh(Vertex[] vertices, uint[] indices, Submesh[] _submeshes, Material[] _materials)
    {
        submeshes = _submeshes;
        materials = _materials;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        // Загружаем ВСЕ вершины модели в один VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<Vertex>(), vertices, BufferUsageHint.StaticDraw);

        // Загружаем ВСЕ индексы модели в один EBO
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Настройка атрибутов (остается как у вас)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);
    }
    public void Draw(/*MaterialManager materialManager*/)
    {
        GL.BindVertexArray(_vao);

        foreach (var submesh in submeshes)
        {
            nint pointerOffset = (nint)(submesh.StartIndex * sizeof(uint));
            Scene.shader.SetVector3("diffuse", materials[submesh.MaterialId].Diffuse);
            Scene.shader.SetVector3("ambient", materials[submesh.MaterialId].Ambient);
            Scene.shader.SetVector3("specular", materials[submesh.MaterialId].Specular);
            Scene.shader.SetFloat("alpha", materials[submesh.MaterialId].Transparency);
            
            // 3. Рисуем только часть индексов
            GL.DrawElements(
                PrimitiveType.Triangles,
                submesh.IndexCount,          // Сколько индексов нарисовать
                DrawElementsType.UnsignedInt,
                pointerOffset                // Откуда начать в EBO (в байтах!)
            );
        }

        GL.BindVertexArray(0);
    }
}
