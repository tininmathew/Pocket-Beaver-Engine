using OpenTK.Mathematics;

namespace Engine.Scripts;

public class Text : Script
{
    public string text;
    public float fontSize;
    public List<GameObject> letters = new();
    public Text(string Text, float size = 50)
    {
        text = Text.ToLower();
        fontSize = size;
    }
    internal override void Start()
    {
        
    }
    public void ChangeText()
    {
        for(int i = 0; i < text.Length; i++)
        {
            GameObject go = new GameObject
            (
                text[i]+i.ToString(), 
                ObjParser.LoadMesh($"models/text/{text[i]}.obj", MeshType.Solid),
                scene, 
                position: gameObject.Transform.Position + new Vector3(i*fontSize, 0, 0),
                scale: new Vector3(-fontSize, fontSize*1.2f, 0),
                parent: gameObject.Transform
            );
            go.Mesh.type = MeshType.UI;
            letters.Add(go);
        }
    }
}