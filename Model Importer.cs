using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using OpenTK.Mathematics;
using Engine.Graphics;

namespace Engine;

public class ObjParser
{
    private List<uint> VertexIndices = new List<uint>();
    private List<Vertex> Vertices = new List<Vertex>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector3> positions = new List<Vector3>();

    // Список для накопления субмешей
    private List<Submesh> Submeshes = new List<Submesh>();
    
    // Перевод текстовых имен материалов из .obj в числовые ID для движка
    private Dictionary<string, int> MaterialNameToId = new Dictionary<string, int>();
    private int _nextMaterialId = 0;

    // Временные переменные для сборки текущего субмеша прямо во время чтения файла
    private int _currentMaterialId = -1;
    private int _currentSubmeshStartIndex = 0;
    private int _currentSubmeshIndexCount = 0;

    void Loader(string filePath)
    {
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
                        normals.Add(new Vector3(xn, yn, zn));
                        break;
                    case "v":
                        float x = float.Parse(parts[1], culture);
                        float y = float.Parse(parts[2], culture);
                        float z = float.Parse(parts[3], culture);
                        positions.Add(new Vector3(x, y, z));
                        break;

                    case "usemtl":
                        string matName = parts[1];

                        // Если мы уже что-то читали для предыдущего материала, сохраняем этот субмеш
                        EndCurrentSubmesh();

                        // Получаем или создаем числовой ID для этого материала
                        if (!MaterialNameToId.TryGetValue(matName, out int matId))
                        {
                            matId = _nextMaterialId++;
                            MaterialNameToId[matName] = matId;
                        }

                        // Настраиваем параметры для начала НОВОГО субмеша
                        _currentMaterialId = matId;
                        _currentSubmeshStartIndex = VertexIndices.Count; // Начинается с текущего конца списка индексов
                        _currentSubmeshIndexCount = 0;                  // Пока в нем 0 индексов
                        break;

                    case "f":
                        // Если в файле по какой-то причине пошли грани ДО первого usemtl, 
                        // создаем дефолтный субмеш с ID = 0, чтобы программа не упала
                        if (_currentMaterialId == -1)
                        {
                            _currentMaterialId = 0;
                            _currentSubmeshStartIndex = 0;
                            _currentSubmeshIndexCount = 0;
                        }

                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] data = parts[i].Split('/');

                            int posIndex = int.Parse(data[0]) - 1;
                            int normIndex = int.Parse(data[2]) - 1;

                            Vertex vertex = new();
                            vertex.position = positions[posIndex];
                            vertex.normal = normals[normIndex];

                            Vertices.Add(vertex);
                            VertexIndices.Add((uint)(Vertices.Count - 1));

                            // Увеличиваем счетчик индексов для ТЕКУЩЕГО субмеша
                            _currentSubmeshIndexCount++;
                        }
                        break;
                }
            }

            // Не забываем сохранить самый последний субмеш после выхода из цикла чтения файла
            EndCurrentSubmesh();
        }
    }

    // Вспомогательный метод: запечатывает текущий субмеш и добавляет его в список
    private void EndCurrentSubmesh()
    {
        if (_currentSubmeshIndexCount > 0)
        {
            Submesh submesh = new Submesh
            {
                StartIndex = _currentSubmeshStartIndex,
                IndexCount = _currentSubmeshIndexCount,
                MaterialId = _currentMaterialId
            };
            Submeshes.Add(submesh);
        }
    }

    public Mesh LoadMesh(string path)
    {
        VertexIndices = new List<uint>();
        Vertices = new List<Vertex>();
        normals = new List<Vector3>();
        positions = new List<Vector3>();
        
        Submeshes = new List<Submesh>();
        MaterialNameToId = new Dictionary<string, int>();
        _nextMaterialId = 0;
        _currentMaterialId = -1;

        Loader(path);

        // Передаем в ваш обновленный Mesh массивы вершин, индексов и сгенерированных субмешей
        return new Mesh(Vertices.ToArray(), VertexIndices.ToArray(), Submeshes.ToArray());
    }
}
