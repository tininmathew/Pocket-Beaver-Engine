using OpenTK.Mathematics;

namespace Engine;

public class Scene
{
    public List<GameObject> List {get;set;} = new List<GameObject>();
    public static Shader shader;
    private List<GameObject> _objectsToRemove = new List<GameObject>();
    public GameObject? Selected { get; private set; }

    public void Add(GameObject toAdd)
    {
        bool isNameInList = false;
        string startName = toAdd.Name;
        for(int i = 0; true; i++)
        {
            foreach(GameObject x in List)
            {
                if(toAdd.Name == x.Name)
                {
                    isNameInList = true;
                    break;
                }
            }
            if(!isNameInList)
            {
                List.Add(toAdd);
                break;
            }
            else
            {
                toAdd.Name = startName + i.ToString();
            }
        }
        var sortedList = List.OrderBy(x => x.Mesh?.IsTransparent).ToList();
        this.List = sortedList;
    }
    public GameObject Find(string name)
    {
        int foundIndex = -1;
        for(int i = 0; i < List.Count; i++)
        {
            if(List[i].Name == name)
            {
                foundIndex = i;
                break;
            }
        }
        if(foundIndex >= 0)
        {
            return List[foundIndex];
        }
        else
        {
            Console.WriteLine("Error: Object not found!");
            return List[0];
        }
    }
    List<Material> originalMaterials = new();
    Material selectedMat = new Material("selected", dif: new Vector3(1,0,0));
    public void Deselect()
    {
        if(Selected==null) return;
        if(Selected.Mesh != null)
        {
            Selected.Mesh.materials = originalMaterials.ToArray();
        }
        Selected = null;
    }
    public void Select(GameObject to)
    {
        if(Selected != null) Deselect();
        Selected = to;
        originalMaterials = new();
        if(Selected == null) return;
        if(Selected.Mesh == null) return;

        for(int i = 0; i < Selected.Mesh.materials.Length; i++)
        {
            originalMaterials.Add(Selected.Mesh.materials[i]);
            Selected.Mesh.materials[i] = selectedMat;
        }
    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject != null && !_objectsToRemove.Contains(gameObject))
        {
            _objectsToRemove.Add(gameObject);
        }
    }

    public void Cleanup()
    {
        if (_objectsToRemove.Count == 0) return;

        foreach (var obj in _objectsToRemove)
        {
            // 1. Удаляем из основного списка сцены
            List.Remove(obj);

            // 2. Освобождаем текстуры, VBO, VAO
            obj.Dispose(); 
        }

        // Очищаем буфер удаления
        _objectsToRemove.Clear();
    }
}