using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{

    List<Vector3> history = new List<Vector3>();
    Vector3 location = new Vector3(0, 0, -5.0f);
    Vector3 velocity = new Vector3(0.05f, 0.06f);
    Vector3 acceleration = new Vector3();
    float maxforce = 0.005f;    // Maximum steering force
    float maxspeed = 0.1f;    // Maximum speed
    GameObject lineObject;
    float left = -5.0f;
    float right = 5.0f;
    float up = 4.0f;
    float down = -4.0f;

    // Use this for initialization
    void Start()
    {

       

        GameObject.Find("Circle").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Triangle").GetComponent<Renderer>().enabled = true;
        lineObject = new GameObject("Line");
        lineObject.name = "Line";
        var line = lineObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;

       
        var borderObj = new GameObject("Border");
        borderObj.name = "Border";
        var border = borderObj.AddComponent<LineRenderer>();
        border.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        border.startColor = Color.blue;
        border.endColor = Color.blue; 
        border.startWidth = 0.02f;
        border.endWidth = 0.02f;
        border.positionCount = 5;
        border.SetPosition(0, new Vector3(left, down, -0.5f));
        border.SetPosition(1, new Vector3(left, up, -0.5f));
        border.SetPosition(2, new Vector3(right, up, -0.5f));
        border.SetPosition(3, new Vector3(right, down, -0.5f));
        border.SetPosition(4, new Vector3(left, down, -0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desired = new Vector3(-500, -500);

        
        if (location.x < left)
        {
            desired = new Vector3(maxspeed, velocity.y);
        }
        else if (location.x > right)
        {
            desired = new Vector3(-maxspeed, velocity.y);
        }

        if (location.y < down)
        {
            desired = new Vector3(velocity.x, maxspeed);
        }
        else if (location.y > up)
        {
            desired = new Vector3(velocity.x, -maxspeed);
        }

        if (desired.x > -500)
        {
            desired.Normalize();
            desired *= maxspeed;
            var steer = desired - velocity;
            steer = Vector3.ClampMagnitude(steer, maxforce);
            acceleration += steer;
        }

        // Update velocity
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxspeed);
        location += velocity;
        // Reset accelerationelertion to 0 each cycle
        acceleration *= 0;

        history.Add(new Vector3(location.x, location.y, -5.0f));
        if (history.Count > 500)
        {
            history.RemoveAt(0);//remove old line
       
        }

        if (history.Count > 1)
        {
            var line = lineObject.GetComponent<LineRenderer>();
           
            line.positionCount = history.Count;

            //staring point and end point
            for (int i = 0; i < history.Count; i++)
            {
                line.SetPosition(i, history[i]);
            }
        }

        GameObject.Find("Triangle").transform.position = new Vector3(location.x, location.y, -5.0f);
        float rot = Vector3.Angle(velocity, Vector3.up);
        //when velocity is in first, forth  quadrants, the culculation of angle is not clockwise, min angle. 2pi-rot
        GameObject.Find("Triangle").transform.rotation = Quaternion.AngleAxis(velocity.x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//计算三角形的旋转角度
        //Debug.Log(velocity  +" "+Vector3.Angle(velocity, Vector3.right));
    }
}
