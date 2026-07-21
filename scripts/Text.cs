using OpenTK.Mathematics;

namespace Engine.Scripts;

public class Text : Script
{
    string text;
    public float fontSize;
    public List<GameObject> letters = new();
    public string TEXT
    {
        get
        {
            return text;
        }
        set
        {
            if(value == text) return;
            Console.WriteLine(value);
            text = value;
            ChangeText();
        }
    }
    public Text(string Text, float size = 50)
    {
        text = Text;
        fontSize = size;
    }
    internal override void Start()
    {
        ChangeText();
    }
    public void ChangeText()
    {
        for(int i = 0; i < letters.Count; i++)
        {
            letters[i].Destroy();
        }
        letters.Clear();
        for(int i = 0; i < text.Length; i++)
        {
            if(text[i] != ' ')
            {
                GameObject go = new GameObject
                (
                    $"{gameObject.Name}-{text[i]}:{i}",
                    ObjParser.LoadMesh($"resources/font/{text[i]}.png", MeshType.UI),
                    scene,
                    position: new Vector3(i*fontSize*0.56f, 0, -1f),
                    scale: new Vector3(fontSize*0.56f, fontSize, 1),
                    parent: gameObject.Transform
                );
                Console.WriteLine(letters.Count);
                letters.Add(go);
            }
        }
    }
}