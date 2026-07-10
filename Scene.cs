namespace Engine;

public class Scene
{
    public List<GameObject> List {get;set;} = new List<GameObject>();
    public static Shader shader;

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
                toAdd.Name = startName + i;
            }
        }
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
}