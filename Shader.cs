using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine;

public class Shader 
{
    internal int currentShaderProgram;
    internal Dictionary<MeshType, int> shaderPrograms = new();

    internal Shader() 
    {
        CompileShaders();
    }

    internal void CompileShaders() 
    {
        string[] VertexShaderSources = { 
            File.ReadAllText("./shaders/solidShader.vert"),
            File.ReadAllText("./shaders/UIShader.vert")
        };
        string[] FragmentShaderSources = { 
            File.ReadAllText("./shaders/solidShader.frag"), 
            File.ReadAllText("./shaders/spriteShader.frag") 
        };

        List<int> VertexShaders = new();
        List<int> FragmentShaders = new();
        int success = 0;

        // вертексные шейдеры
        foreach(string i in VertexShaderSources) 
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, i);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out success);
            if (success == 0) 
            {
                Console.WriteLine($"Vertex Shader Error: {GL.GetShaderInfoLog(vertexShader)}");
            }
            VertexShaders.Add(vertexShader);
        }

        // фрагментные шейдеры
        foreach(string i in FragmentShaderSources) 
        {
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, i);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0) 
            {
                Console.WriteLine($"Fragment Shader Error: {GL.GetShaderInfoLog(fragmentShader)}");
            }
            FragmentShaders.Add(fragmentShader);
        }

        // програмы
        int meshTypeCount = Enum.GetValues(typeof(MeshType)).Length;
        for(int i = 0; i < meshTypeCount; i++) 
        {
            int _shaderProgram = GL.CreateProgram();
            switch((MeshType)i) 
            {
                case MeshType.Solid:
                    GL.AttachShader(_shaderProgram, VertexShaders[0]);
                    GL.AttachShader(_shaderProgram, FragmentShaders[0]);
                    break;
                case MeshType.Sprite:
                    GL.AttachShader(_shaderProgram, VertexShaders[0]);
                    GL.AttachShader(_shaderProgram, FragmentShaders[1]);
                    break;
                case MeshType.UI:
                    GL.AttachShader(_shaderProgram, VertexShaders[1]);
                    GL.AttachShader(_shaderProgram, FragmentShaders[1]);
                    break;
            }

            GL.LinkProgram(_shaderProgram);
            GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out success);
            if (success == 0) 
            {
                Console.WriteLine($"Program Link Error ({(MeshType)i}): {GL.GetProgramInfoLog(_shaderProgram)}");
            }

            Console.WriteLine($"{(MeshType)i} Program ID: {_shaderProgram}");
            shaderPrograms.Add((MeshType)i, _shaderProgram);
        }

        foreach(int shader in VertexShaders) { GL.DeleteShader(shader); }
        foreach(int shader in FragmentShaders) { GL.DeleteShader(shader); }
        foreach (var program in shaderPrograms)
        {
            GL.UseProgram(program.Value);
            int location = GL.GetUniformLocation(program.Value, "texture0");
            if (location != -1)
            {
                GL.Uniform1(location, 0); 
            }
        }
        GL.UseProgram(0);
    }

    internal void Use(MeshType type) 
    {
        if (shaderPrograms.TryGetValue(type, out int program))
        {
            currentShaderProgram = program;
            GL.UseProgram(currentShaderProgram);
        }
    }

    public void SetFloat(string name, float value) 
    {
        int location = GL.GetUniformLocation(currentShaderProgram, name);
        GL.Uniform1(location, value);
    }

    public void SetVector3(string name, Vector3 value) 
    {
        int location = GL.GetUniformLocation(currentShaderProgram, name);
        GL.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetMatrix4(string name, Matrix4 matrix) 
    {
        int location = GL.GetUniformLocation(currentShaderProgram, name);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector3Array(string name, Vector3[] value) 
    {
        float[] floatArray = new float[value.Length * 3];
        for (int i = 0; i < value.Length; i++) 
        {
            floatArray[i * 3 + 0] = value[i].X;
            floatArray[i * 3 + 1] = value[i].Y;
            floatArray[i * 3 + 2] = value[i].Z;
        }
        int location = GL.GetUniformLocation(currentShaderProgram, name);
        GL.Uniform3(location, value.Length, floatArray);
    }

    public void SetLights(PointLight[] lights) 
    {
        SetVector3("dirLight.transform", DirLight.Rotation);
        SetVector3("dirLight.color", DirLight.Color);
        SetFloat("dirLight.intensity", DirLight.Intensity);

        for (int i = 0; i < lights.Length; i++) 
        {
            SetVector3($"lights[{i}].transform", lights[i].Transform.Position);
            SetVector3($"lights[{i}].color", lights[i].Color);
            SetFloat($"lights[{i}].intensity", lights[i].Intensity);
        }
    }
}
