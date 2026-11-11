using UnityEngine;

public class PrototypeSpawner : MonoBehaviour
{
    public Material blueCarMaterial, orangeCarMaterial, ballMaterial, fieldMaterial, wallMaterial;

    void Start()
    {
        GameObject arena = new GameObject("Procedural Arena");
        ProceduralArena builder = arena.AddComponent<ProceduralArena>();
        builder.fieldMaterial=fieldMaterial; builder.wallMaterial=wallMaterial;
        GameObject ball=CreateBall();
        GameObject player=CreateCar("Player Car",new Vector3(0f,1f,-9f),0f,blueCarMaterial);
        CreateCar("Practice Car",new Vector3(0f,1f,9f),180f,orangeCarMaterial);

        Camera cam=Camera.main;
        if(cam==null) { GameObject c=new GameObject("Main Camera"); c.tag="MainCamera"; cam=c.AddComponent<Camera>(); }
        FollowCamera follow=cam.GetComponent<FollowCamera>();
        if(follow==null) follow=cam.gameObject.AddComponent<FollowCamera>();
        follow.target=player.transform;
        cam.transform.position=player.transform.position+new Vector3(0f,5.5f,-8f);

        new GameObject("Match Manager").AddComponent<MatchManager>();
        SimpleTouchHUD hud=new GameObject("Touch HUD").AddComponent<SimpleTouchHUD>();
        hud.targetCar=player.GetComponent<CarController>();
        ball.transform.position=new Vector3(0f,1.2f,0f);
    }

    GameObject CreateBall()
    {
        GameObject ball=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name="Match Ball"; ball.tag="Ball"; ball.transform.localScale=Vector3.one*2f;
        if(ballMaterial!=null) ball.GetComponent<Renderer>().material=ballMaterial;
        Rigidbody rb=ball.AddComponent<Rigidbody>();
        rb.mass=2f; rb.linearDamping=0.15f; rb.angularDamping=0.08f;
        rb.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
        return ball;
    }

    GameObject CreateCar(string n, Vector3 p, float yaw, Material mat)
    {
        GameObject car=new GameObject(n); car.transform.position=p; car.transform.rotation=Quaternion.Euler(0f,yaw,0f);
        BoxCollider box=car.AddComponent<BoxCollider>(); box.center=new Vector3(0f,0.55f,0f); box.size=new Vector3(1.8f,0.8f,3f);
        Rigidbody rb=car.AddComponent<Rigidbody>(); rb.mass=1100f; rb.linearDamping=0.4f; rb.angularDamping=2f;
        rb.interpolation=RigidbodyInterpolation.Interpolate;
        car.AddComponent<CarController>();

        Visual(car,"Body",new Vector3(0f,0.55f,0f),new Vector3(1.8f,0.65f,3f),PrimitiveType.Cube,mat);
        Visual(car,"Cabin",new Vector3(0f,1.05f,-0.15f),new Vector3(1.25f,0.55f,1.45f),PrimitiveType.Cube,mat);
        Vector3[] wheels={new Vector3(-0.95f,0.28f,1f),new Vector3(0.95f,0.28f,1f),new Vector3(-0.95f,0.28f,-1f),new Vector3(0.95f,0.28f,-1f)};
        foreach(Vector3 wp in wheels) Visual(car,"Wheel",wp,new Vector3(0.55f,0.22f,0.55f),PrimitiveType.Cylinder,null).transform.localRotation=Quaternion.Euler(0f,0f,90f);
        return car;
    }

    GameObject Visual(GameObject parent,string n,Vector3 p,Vector3 s,PrimitiveType type,Material mat)
    {
        GameObject o=GameObject.CreatePrimitive(type); o.name=n; o.transform.SetParent(parent.transform,false);
        o.transform.localPosition=p; o.transform.localScale=s;
        Collider c=o.GetComponent<Collider>(); if(c!=null) Destroy(c);
        if(mat!=null) o.GetComponent<Renderer>().material=mat;
        return o;
    }
}