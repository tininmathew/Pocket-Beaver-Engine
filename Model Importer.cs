using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using OpenTK.Mathematics;
using Engine.Graphics;

namespace Engine;

public class ObjParser
{
    List<uint> VertexIndices = new List<uint>();

    List<Vertex> Vertices = new List<Vertex>();


    List<Vector3> normals = new List<Vector3>();
    List<Vector3> positions = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();

    void Loader(string filePath)
    {
        // Использование InvariantCulture гарантирует, что дробные числа 
        // с точкой (например, 0.5) будут правильно читаться на любом ПК
        CultureInfo culture = CultureInfo.InvariantCulture;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line == "" || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string type = parts[0];
                switch (type)
                {
                    case "vn":
                        float xn = float.Parse(parts[1], culture);
                        float yn = float.Parse(parts[2], culture);
                        float zn = float.Parse(parts[3], culture);
                        normals.Add(new Vector3(xn,yn,zn));
                        break;
                    case "v":
                        float x = float.Parse(parts[1], culture);
                        float y = float.Parse(parts[2], culture);
                        float z = float.Parse(parts[3], culture);
                        positions.Add(new Vector3(x,y,z));
                        break;
                    /*case "vt":
                        float xt = float.Parse(parts[1], culture);
                        float yt = float.Parse(parts[2], culture);
                        uv.Add(new Vector2(xt, yt));
                        break;*/

                    case "f":
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] data = parts[i].Split('/');

                            int posIndex = int.Parse(data[0]) - 1;
                            int normIndex = int.Parse(data[2]) - 1;

                            Vertex vertex = new();

                            vertex.position = positions[posIndex];
                            vertex.normal = normals[normIndex];

                            Vertices.Add(vertex);

                            VertexIndices.Add(
                                (uint)(Vertices.Count - 1));
                        }

                        break;
                }
            }
        }
    }
    public Mesh LoadMesh(string path)
    {
        VertexIndices = new List<uint>();
        Vertices = new List<Vertex>();
        normals = new List<Vector3>();
        positions = new List<Vector3>();
        uv = new List<Vector2>();

        Loader(path);
        return new Mesh(Vertices.ToArray(), VertexIndices.ToArray());
    }
}
