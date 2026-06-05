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
    public uint[] indices;
    public int materialID;
};

public struct Material
{
    public Vector3 ambient;
    public Vector3 diffuse;
    public Vector3 specular;
    public float roughness;
    public float alpha;

};