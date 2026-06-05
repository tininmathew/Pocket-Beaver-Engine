using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine;



public class Shader
{
    internal int _shaderProgram;
    internal Shader()
    {
        CompileShaders();
    }
    internal void CompileShaders()
    {
        string vertexShaderSource = File.ReadAllText("./shaders/vertexShader.vert");
        string fragmentShaderSource = File.ReadAllText("./shaders/fragmentShader.frag");
        //вертексный шейдер:
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        
        GL.GetShader(vertexShader,
            ShaderParameter.CompileStatus,
            out int success);

        if (success == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
        }

        //фрагментарный шейдер:
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader,
            ShaderParameter.CompileStatus,
            out success);

        if (success == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));
        }

        //программа:
        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        GL.GetShader(_shaderProgram,
            ShaderParameter.CompileStatus,
            out success);

        if (success == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(_shaderProgram));
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }
    internal void Use()
    {
        GL.UseProgram(_shaderProgram);
    }
    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(_shaderProgram, name);

        GL.Uniform3(location, value.X, value.Y, value.Z);
    }
    public void SetMatrix4(string name,Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(_shaderProgram, name);

        GL.UniformMatrix4(location, false, ref matrix);
    }
}
