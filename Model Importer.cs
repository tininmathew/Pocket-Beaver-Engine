using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using OpenTK.Mathematics;

namespace Engine;

public class ObjParser
{
    private List<uint> VertexIndices = new();
    private List<Vertex> Vertices = new();
    private List<Vector3> normals = new();
    private List<Vector3> positions = new();
    public List<Vector2> UVs = new();

    private List<Submesh> Submeshes = new();
    
    // Храним маппинг ИмяМатериала -> ID для синхронизации
    private Dictionary<string, int> MaterialNameToId = new();
    private int _nextMaterialId = 0;

    private int _currentMaterialId = -1; // Изначально -1 (нет материала)
    private int _currentSubmeshStartIndex = 0;
    private int _currentSubmeshIndexCount = 0;
    private string pathToMtl = "";

    private void Loader(string filePath)
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

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                string type = parts[0];
                switch (type)
                {
                    case "mtllib":
                        // Корректное объединение путей для любых ОС
                        pathToMtl = Path.Combine(Path.GetDirectoryName(filePath) ?? "", parts[1]);
                        break;

                    case "vn":
                        normals.Add(new Vector3(
                            float.Parse(parts[1], culture),
                            float.Parse(parts[2], culture),
                            float.Parse(parts[3], culture)
                        ));
                        break;

                    case "v":
                        positions.Add(new Vector3(
                            float.Parse(parts[1], culture),
                            float.Parse(parts[2], culture),
                            float.Parse(parts[3], culture)
                        ));
                        break;
                    case "vt":
                            UVs.Add(new Vector2(
                                float.Parse(parts[1], culture),
                                float.Parse(parts[2], culture)
                            ));
                        break;

                    case "usemtl":
                        string matName = parts[1];

                        // Сохраняем предыдущий субмеш, если в нем были полигоны
                        EndCurrentSubmesh();

                        // Регистрируем материал, если встретили впервые
                        if (!MaterialNameToId.TryGetValue(matName, out int matId))
                        {
                            matId = _nextMaterialId++;
                            MaterialNameToId[matName] = matId;
                        }

                        _currentMaterialId = matId;
                        _currentSubmeshStartIndex = VertexIndices.Count;
                        _currentSubmeshIndexCount = 0;
                        break;

                    case "f":
                        // Защита: если полигоны идут до usemtl
                        if (_currentMaterialId == -1)
                        {
                            string defaultMat = "default_material";
                            if (!MaterialNameToId.TryGetValue(defaultMat, out _currentMaterialId))
                            {
                                _currentMaterialId = _nextMaterialId++;
                                MaterialNameToId[defaultMat] = _currentMaterialId;
                            }
                            _currentSubmeshStartIndex = 0;
                            _currentSubmeshIndexCount = 0;
                        }

                        // Поддержка триангуляции на лету (для полигонов с > 3 вершинами)
                        // Разбиваем веером (Fan Triangulation): вершины (1, 2, 3), (1, 3, 4), (1, 4, 5)...
                        List<uint> faceIndices = new List<uint>();

                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] data = parts[i].Split('/');

                            int posIndex = int.Parse(data[0]);
                            // Обработка отрицательных индексов OBJ и приведение к 0-based
                            posIndex = posIndex < 0 ? positions.Count + posIndex : posIndex - 1;

                            int UVIndex = -1;
                            if (data.Length > 1 && !string.IsNullOrEmpty(data[1]))
                            {
                                UVIndex = int.Parse(data[1]);
                                UVIndex = UVIndex < 0 ? UVs.Count + UVIndex : UVIndex - 1;
                            }
                            int normIndex = -1;
                            if (data.Length > 2 && !string.IsNullOrEmpty(data[2]))
                            {
                                normIndex = int.Parse(data[2]);
                                normIndex = normIndex < 0 ? normals.Count + normIndex : normIndex - 1;
                            }
                            

                            Vertex vertex = new Vertex
                            {
                                position = positions[posIndex],
                                normal = normIndex != -1 ? normals[normIndex] : Vector3.UnitY, // Дефолтная нормаль
                                uv = UVIndex!=-1 ? UVs[UVIndex] : Vector2.Zero
                            };

                            Vertices.Add(vertex);
                            faceIndices.Add((uint)(Vertices.Count - 1));
                        }

                        // Добавляем индексы в буфер в виде треугольников
                        for (int i = 1; i < faceIndices.Count - 1; i++)
                        {
                            VertexIndices.Add(faceIndices[0]);
                            VertexIndices.Add(faceIndices[i]);
                            VertexIndices.Add(faceIndices[i + 1]);
                            _currentSubmeshIndexCount += 3;
                        }
                        break;
                }
            }

            EndCurrentSubmesh();
        }
    }

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

    // Изменено: Передаем Dictionary для синхронизации ID по имени
    private static Material[] LoadMTL(string filePath, Dictionary<string, int> materialNameToId)
    {
        if (!File.Exists(filePath))
        {
            // Если MTL нет, создаем массив заглушек на основе найденных в OBJ имен
            Material[] fallbacks = new Material[materialNameToId.Count];
            foreach (var kvp in materialNameToId)
            {
                fallbacks[kvp.Value] = new Material(kvp.Key);
            }
            return fallbacks;
        }

        // Выделяем массив под точное количество материалов, найденных в OBJ
        Material[] materials = new Material[materialNameToId.Count];
        Material currentMaterial = null;
        int currentId = -1;

        foreach (var line in File.ReadLines(filePath))
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            var parts = trimmedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            switch (parts[0].ToLower())
            {
                case "newmtl":
                    string matName = parts[1];
                    // Проверяем, используется ли этот материал в OBJ
                    if (materialNameToId.TryGetValue(matName, out currentId))
                    {
                        currentMaterial = new Material(matName);
                        materials[currentId] = currentMaterial;
                    }
                    else
                    {
                        currentMaterial = null; // Пропускаем, если в OBJ его нет
                    }
                    break;

                case "ka":
                    if (currentMaterial != null) currentMaterial.Ambient = ParseVector3(parts);
                    break;

                case "kd":
                    if (currentMaterial != null) currentMaterial.Diffuse = ParseVector3(parts);
                    break;

                case "ks":
                    if (currentMaterial != null) currentMaterial.Specular = ParseVector3(parts);
                    break;

                case "d":
                    if (currentMaterial != null) currentMaterial.Transparency = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    break;

                case "map_kd":
                    if (currentMaterial != null) currentMaterial.Texture = TextureLoader.LoadTexture(parts[1]);
                    break;
            }
        }

        // Заполняем пропуски дефолтными материалами (если в MTL не было описания)
        foreach (var kvp in materialNameToId)
        {
            if (materials[kvp.Value] == null)
            {
                materials[kvp.Value] = new Material(kvp.Value == 0 ? "default_material" : kvp.Key);
            }
        }

        return materials;
    }

    private static Vector3 ParseVector3(string[] parts)
    {
        return new Vector3
        (
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture),
            float.Parse(parts[3], CultureInfo.InvariantCulture)
        );
    }

    public Mesh LoadMesh(string path)
    {
        // Полный сброс состояния для повторного использования парсера
        VertexIndices.Clear();
        Vertices.Clear();
        normals.Clear();
        positions.Clear();
        UVs.Clear();
        Submeshes.Clear();
        MaterialNameToId.Clear();
        
        _nextMaterialId = 0;
        _currentMaterialId = -1; 
        _currentSubmeshStartIndex = 0;
        _currentSubmeshIndexCount = 0;
        pathToMtl = "";

        Loader(path);
        
        // Передаем карту имен в LoadMTL для строгого соответствия индексов
        Material[] materials = LoadMTL(pathToMtl, MaterialNameToId); 

        return new Mesh(
            Vertices.ToArray(), 
            VertexIndices.ToArray(), 
            Submeshes.ToArray(), 
            materials
        );
    }
}
