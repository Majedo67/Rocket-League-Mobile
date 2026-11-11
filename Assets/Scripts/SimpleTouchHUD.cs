using UnityEngine;

public class SimpleTouchHUD : MonoBehaviour
{
    public CarController targetCar;
    GUIStyle buttonStyle, labelStyle;

    void OnGUI()
    {
        if(targetCar==null) return;
        if(buttonStyle==null) { buttonStyle=new GUIStyle(GUI.skin.button); buttonStyle.fontSize=Mathf.Max(18,Screen.height/35); buttonStyle.fontStyle=FontStyle.Bold; }
        if(labelStyle==null) { labelStyle=new GUIStyle(GUI.skin.label); labelStyle.fontSize=Mathf.Max(18,Screen.height/35); labelStyle.normal.textColor=Color.white; }

        float u=Mathf.Clamp(Screen.width*0.16f,90f,155f), y=Screen.height-u*1.45f;
        Hold("LEFT",new Rect(18f,y,u,u),-1f,true);
        Hold("RIGHT",new Rect(28f+u,y,u,u),1f,true);
        Hold("GO",new Rect(Screen.width-u*2.15f,y,u,u),1f,false);
        Hold("REV",new Rect(Screen.width-u*1.05f,y,u,u),-1f,false);
        targetCar.boostPressed=GUI.RepeatButton(new Rect(Screen.width-u*1.05f,y-u*1.1f,u,u*0.8f),"BOOST",buttonStyle);
        GUI.Label(new Rect(18f,15f,400f,45f),"CAR SOCCER PROTOTYPE",labelStyle);
        GUI.Label(new Rect(18f,55f,250f,40f),"BOOST: "+Mathf.RoundToInt(targetCar.BoostAmount*100f)+"%",labelStyle);
    }

    void Hold(string title,Rect rect,float value,bool steering)
    {
        bool down=GUI.RepeatButton(rect,title,buttonStyle);
        if(steering)
        {
            if(down) targetCar.steering=value;
            else if(Mathf.Sign(targetCar.steering)==Mathf.Sign(value)) targetCar.steering=0f;
        }
        else
        {
            if(down) targetCar.throttle=value;
            else if(Mathf.Sign(targetCar.throttle)==Mathf.Sign(value)) targetCar.throttle=0f;
        }
    }
}