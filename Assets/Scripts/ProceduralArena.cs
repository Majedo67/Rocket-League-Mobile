using UnityEngine;

public class ProceduralArena : MonoBehaviour
{
    public float length = 46f;
    public float width = 30f;
    public float wallHeight = 3f;
    public float wallThickness = 1f;
    public Material fieldMaterial, wallMaterial, blueMaterial, orangeMaterial;

    void Start() { Build(); }

    public void Build()
    {
        Block("Field", new Vector3(0f,-0.5f,0f), new Vector3(width,1f,length), fieldMaterial);
        Block("North Wall", new Vector3(0f,wallHeight/2f,length/2f), new Vector3(width,wallHeight,wallThickness), wallMaterial);
        Block("South Wall", new Vector3(0f,wallHeight/2f,-length/2f), new Vector3(width,wallHeight,wallThickness), wallMaterial);
        Block("East Wall", new Vector3(width/2f,wallHeight/2f,0f), new Vector3(wallThickness,wallHeight,length), wallMaterial);
        Block("West Wall", new Vector3(-width/2f,wallHeight/2f,0f), new Vector3(wallThickness,wallHeight,length), wallMaterial);
        Goal("Blue Goal", new Vector3(0f,1.2f,-length/2f+1.2f), blueMaterial, 0);
        Goal("Orange Goal", new Vector3(0f,1.2f,length/2f-1.2f), orangeMaterial, 1);
        Block("Center Stripe", Vector3.zero, new Vector3(width-2f,0.06f,0.12f), null);
    }

    void Goal(string goalName, Vector3 pos, Material mat, int team)
    {
        GameObject goal = new GameObject(goalName);
        goal.transform.SetParent(transform);
        goal.transform.position = pos;
        BoxCollider trigger = goal.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(10f,4f,4f);
        goal.AddComponent<GoalTrigger>().scoringTeam = team;
        Block(goalName+" Back", pos + new Vector3(0f,0f,team==0?-1.8f:1.8f), new Vector3(10f,3f,0.3f), mat);
        Block(goalName+" Left Post", pos + new Vector3(-5f,1.5f,0f), new Vector3(0.3f,3f,3.5f), mat);
        Block(goalName+" Right Post", pos + new Vector3(5f,1.5f,0f), new Vector3(0.3f,3f,3.5f), mat);
        Block(goalName+" Crossbar", pos + new Vector3(0f,3f,0f), new Vector3(10f,0.3f,3.5f), mat);
    }

    void Block(string n, Vector3 p, Vector3 s, Material m)
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name=n; o.transform.SetParent(transform); o.transform.position=p; o.transform.localScale=s;
        if (m != null) o.GetComponent<Renderer>().material=m;
    }
}