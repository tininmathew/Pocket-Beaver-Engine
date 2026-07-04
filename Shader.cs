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
    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(_shaderProgram, name);
        GL.Uniform1(location, value);
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
    public void SetVector3Array(string name, Vector3[] value)
    {
        float[] floatArray = new float[value.Length * 3];
        for (int i = 0; i < value.Length; i++)
        {
            floatArray[i * 3 + 0] = value[i].X;
            floatArray[i * 3 + 1] = value[i].Y;
            floatArray[i * 3 + 2] = value[i].Z;
        }
        int location = GL.GetUniformLocation(_shaderProgram, name);
        GL.Uniform3(location, value.Length, floatArray);
    }
    public void SetLights(PointLight[] lights)
    {
        SetVector3($"dirLight.transform", DirLight.Rotation);
        SetVector3($"dirLight.color", DirLight.Color);
        SetFloat($"dirLight.intensity", DirLight.Intensity);
        for (int i = 0; i < lights.Length; i++)
        {
            SetVector3($"lights[{i}].transform", lights[i].Transform.Position);
            SetVector3($"lights[{i}].color", lights[i].Color);
            SetFloat($"lights[{i}].intensity", lights[i].Intensity);
        }
    }
}
