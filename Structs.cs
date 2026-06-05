using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Engine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vertex
{
    public Vector3 position;
    public Vector3 normal;
    //public Vector2 uv;
};