using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Engine.Graphics;

public class Mesh
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;

    private readonly int _indexCount;

    public Mesh(Vertex[] vertices, uint[] indices, nint debug_Offset = 3)
    {
        _indexCount = indices.Length;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(
            BufferTarget.ArrayBuffer,vertices.Length * Marshal.SizeOf<Vertex>(),
            vertices,
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(
            BufferTarget.ElementArrayBuffer,
            _ebo);

        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            indices.Length * sizeof(uint),
            indices,
            BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(
            0,
            3,
            VertexAttribPointerType.Float,
            false,
            Marshal.SizeOf<Vertex>(),
            0);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(
            1,
            3,
            VertexAttribPointerType.Float,
            false,
            Marshal.SizeOf<Vertex>(),
            3 * sizeof(float));

        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);

        GL.DrawElements(
            PrimitiveType.Triangles,
            _indexCount,
            DrawElementsType.UnsignedInt,
            0);
    }
}