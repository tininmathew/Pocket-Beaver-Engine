using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Engine.Graphics;

public class Mesh
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;
    
    // Список всех подмешей
    private readonly Submesh[] _submeshes;

    public Mesh(Vertex[] vertices, uint[] indices, Submesh[] submeshes)
    {
        _submeshes = submeshes;

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

        GL.BindVertexArray(0);
    }

    // Теперь методу Draw нужно знать про систему материалов вашего движка,
    // либо вы можете просто вызывать рендер конкретного субмеша
    public void Draw(/*MaterialManager materialManager*/)
    {
        GL.BindVertexArray(_vao);

        foreach (var submesh in _submeshes)
        {
            // 1. Включаем материал для текущего субмеша (активируем шейдер, текстуры)
            //materialManager.UseMaterial(submesh.MaterialId);

            // 2. Считаем смещение в байтах внутри EBO
            // Системный указатель должен указывать на байт, с которого начинается сабмеш
            nint pointerOffset = (nint)(submesh.StartIndex * sizeof(uint));

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
