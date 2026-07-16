using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Engine;

public enum MeshType
{
    Solid,
    Sprite
}

public class Mesh : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;
    
    public MeshType type { get; private set; }
    public Submesh[] submeshes;
    public Material[] materials;
    public readonly bool IsTransparent = false;
    private bool _disposed = false;
    private List<Submesh> opaqueSubmeshes = new();
    private List<Submesh> transparentSubmeshes = new();


    public Mesh(Vertex[] vertices, uint[] indices, Submesh[] _submeshes, Material[] _materials, MeshType type_)
    {
        type = type_;
        submeshes = _submeshes;
        materials = _materials;
        foreach(Material i in materials)
        {
            if(i.Transparency < 1)
            {
                IsTransparent = true;
                break;
            }
        }
        for(int i = 0; i < submeshes.Length; i++)
        {
            if(materials[submeshes[i].MaterialId].Transparency < 1)
            {
                transparentSubmeshes.Add(submeshes[i]);
            }
            else
            {
                opaqueSubmeshes.Add(submeshes[i]);
            }
        }

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
    public void Draw(Matrix4 model, string debug)
    {
        Scene.shader.Use(type);
        Scene.shader.SetMatrix4("model", model, debug);
        GL.BindVertexArray(_vao);
        if(type == MeshType.Solid)
        {
            bool opaqueRendered = false;
            for(int i = 0; i < 2; i ++)
            {
                foreach(var submesh in opaqueRendered?transparentSubmeshes:opaqueSubmeshes)
                {
                    nint pointerOffset = (nint)(submesh.StartIndex * sizeof(uint));
                    Scene.shader.SetVector3("diffuse", materials[submesh.MaterialId].Diffuse);
                    Scene.shader.SetVector3("ambient", materials[submesh.MaterialId].Ambient);
                    Scene.shader.SetVector3("specular", materials[submesh.MaterialId].Specular);
                    Scene.shader.SetFloat("alpha", materials[submesh.MaterialId].Transparency);
                    if(materials[submesh.MaterialId].Texture != -1)
                    {
                        int handle = materials[submesh.MaterialId].Texture;
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, handle);
                    }
                    if(opaqueRendered)
                    {
                        GL.Enable(EnableCap.Blend);
                        GL.DepthMask(false);
                    }
                        
                    
                    // 3. Рисуем только часть индексов
                    GL.DrawElements(
                        PrimitiveType.Triangles,
                        submesh.IndexCount,          // Сколько индексов нарисовать
                        DrawElementsType.UnsignedInt,
                        pointerOffset                // Откуда начать в EBO (в байтах!)
                    );
                    GL.DepthMask(true);
                    GL.Disable(EnableCap.Blend);
                }
                opaqueRendered = true;
            }
        }
        else
        {
            Submesh submesh = submeshes[0];
            nint pointerOffset = (nint)(submesh.StartIndex * sizeof(uint));
            
            Scene.shader.SetVector3("diffuse", materials[submesh.MaterialId].Diffuse);
            Scene.shader.SetVector3("ambient", materials[submesh.MaterialId].Ambient);
            Scene.shader.SetVector3("specular", materials[submesh.MaterialId].Specular);
            Scene.shader.SetFloat("alpha", materials[submesh.MaterialId].Transparency);
            
            int handle = materials[submesh.MaterialId].Texture;
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            
            // Отключаем отсечение, так как билборд должен быть виден с обеих сторон
            GL.Disable(EnableCap.CullFace); 
            
            // ИСПРАВЛЕНО: Раскомментируйте блендинг, так как спрайты обычно имеют прозрачные области
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            GL.DrawElements(
                PrimitiveType.Triangles,
                submesh.IndexCount,
                DrawElementsType.UnsignedInt,
                pointerOffset
            );
            
            // ИСПРАВЛЕНО: Возвращаем состояние контекста OpenGL в исходное
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace); // Включаем обратно для Solid объектов!
        }
        
        GL.BindVertexArray(0);
        
    }


    //dispose logic:
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            // Важно: удаление ресурсов OpenGL должно происходить в главном потоке рендеринга!
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);

            _disposed = true;
        }
    }

    ~Mesh()
    {
        // Предупреждение: OpenGL команды здесь могут вызвать краш, 
        // если финализатор вызовется из другого потока.
        // Поэтому всегда вызывайте Dispose вручную.
    }
}
