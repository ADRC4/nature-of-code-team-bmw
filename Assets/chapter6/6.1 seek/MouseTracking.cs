using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour {

    List<Vector3> history = new List<Vector3>();
    bool showCircle = false;
    Vector3 location = new Vector3(0, 0, -5.0f);
    Vector3 velocity = new Vector3();
    Vector3 acceleration = new Vector3();
    float maxforce = 0.1f;    // Maximum steering force
    float maxspeed = 4;    // Maximum speed
    GameObject lineObject;
    Vector3 mouse = new Vector3();
    bool running = false;

    // Use this for initialization
    void Start () {

        //edge for trangle and circle，failed，keep them for later edition

        GameObject.Find("Circle").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Triangle").GetComponent<Renderer>().enabled = false;
        lineObject = new GameObject("Line");
        lineObject.name = "Line";
        lineObject.AddComponent<LineRenderer>();//for drawing lines
    }

    // Update is called once per frame
    void Update () {

        if (!running)
            return;

        // Update velocity
        velocity += acceleration;//calculate the speed
        velocity = Vector3.ClampMagnitude(velocity, maxspeed);//set upper limit
        location += velocity;
        // Reset accelerationelertion to 0 each cycle
        acceleration *= 0;

        history.Add(new Vector3(location.x, location.y, -5.0f));//add the trail，-5 is the z height，lower when number gets bigger.
        if (history.Count > 100)
        {
            history.RemoveAt(0);//REMOVE THE earliest lines
        }

        if (history.Count > 1)
        {
            var line = lineObject.GetComponent<LineRenderer>();
            // setColor for using material
            line.material = new Material(Shader.Find("Particles/Additive"));
            line.positionCount = history.Count;//set 2points
            line.startColor = Color.black;    //straight！ the trail should be a black one at the very beginning
            line.endColor = Color.grey;       //set the color of trail
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;            //font of trail

            //set the start and end point of the trail
            for (int i = 0; i < history.Count; i++)
            {
                line.SetPosition(i, history[i]);//point of trail
            }
        }
        GameObject.Find("Circle").transform.position = new Vector3(mouse.x, mouse.y, -1.0f);
        GameObject.Find("Triangle").transform.position = new Vector3(location.x, location.y, -5.0f);
        float rot = Vector3.Angle(velocity, Vector3.up);
        GameObject.Find("Triangle").transform.rotation = Quaternion.AngleAxis(velocity.x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);
        //Debug.Log(velocity  +" "+Vector3.Angle(velocity, Vector3.right));
    }

    void OnMouseUp()
    {
        running = false;
    }

  
    void OnMouseDrag()
    {
        running = true;
        if (!showCircle)
        {
            showCircle = true;
            GameObject.Find("Circle").GetComponent<Renderer>().enabled = true;
            GameObject.Find("Triangle").GetComponent<Renderer>().enabled = true;
        }

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        //distancebetween trangle and target
        Vector3 desired = mouse - location;  // A vector pointing from the location to the target
        float d = desired.magnitude;
        // Normalize desired and scale with arbitrary damping within 100 pixels
        desired.Normalize();
        if (d < 100)
        {
            float m = d / 100.0f * maxspeed;
            desired *= m;
        }
        else
        {
            desired *= maxspeed;
        }

        // Steering = Desired minus Velocity
        Vector3 steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxforce);  // Limit to maximum steering force
        acceleration += steer;
    }
}
