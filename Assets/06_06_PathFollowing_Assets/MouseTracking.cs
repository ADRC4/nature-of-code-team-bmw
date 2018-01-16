using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{
    static float left = -6.0f; 
    static float right = 6.0f;
    static float up = 4.0f;
    static float down = -4.0f;

    //position of two vehicles
    static float[] maxforce = { 0.002f, 0.012f }; // Maximum steering force
    static float[] maxspeed = { 0.04f, 0.12f };    // Maximum speed
    Vector3[] location = { new Vector3(left, 0, 0), new Vector3(left, 0, 0) };
    Vector3[] velocity = { new Vector3(maxspeed[0], 0), new Vector3(maxspeed[1], 0) };
    Vector3[] acceleration = { new Vector3(), new Vector3() };

    bool debug = true;

    List<Vector3> path = new List<Vector3>();//4points od path

    // Use this for initialization
    void Start()
    {
        path.Add(new Vector3(left, 0));
        path.Add(new Vector3(Random.value * left, Random.value * 2.0f - 1f));//Fluctuation range
        path.Add(new Vector3(Random.value * right, Random.value * 2.0f - 1f));//Fluctuation range
        path.Add(new Vector3(right, 0));
        


        GameObject.Find("Circle").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Triangle").GetComponent<Renderer>().enabled = false;

        {
            for (int i = 0; i < 2; i++)
            {
               
                var tri = Instantiate(GameObject.Find("Triangle"));//vehicle
                tri.GetComponent<Renderer>().enabled = true;
                tri.name = "Triangle" + i;
                var cir = Instantiate(GameObject.Find("Circle"));
                cir.GetComponent<Renderer>().enabled = true;
                cir.name = "Circle" + i;
                cir.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                var cir2 = Instantiate(GameObject.Find("Circle"));
                cir2.GetComponent<Renderer>().enabled = true;
                cir2.name = "CircleT" + i;
                cir2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                var bling = Instantiate(GameObject.Find("Circle"));//blingpoint
                bling.GetComponent<Renderer>().enabled = true;
                bling.name = "Bling" + i;
                bling.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);


                {
                    var borderObj = new GameObject("Trace" + i);
                    borderObj.name = "Trace" + i;
                    borderObj.transform.position = new Vector3(0, 0, -2.4f);
                    var border = borderObj.AddComponent<LineRenderer>();
                    border.material = new Material(Shader.Find("Sprites/Default"));
                    border.startColor = Color.black;
                    border.endColor = Color.black; 
                    border.startWidth = 0.02f;
                    border.endWidth = 0.02f;
                    border.positionCount = 3;
                    border.useWorldSpace = true;
                    //border.SetPosition(0, Vector3.zero);
                    // border.SetPosition(1, Vector3.zero);
                    //border.SetPosition(2, Vector3.zero);
                }
            }
        }

       
        {
            
            var borderObj = new GameObject("Line");
            borderObj.name = "Line";
            borderObj.transform.position = new Vector3(0, 0, -3f);
            var border = borderObj.AddComponent<LineRenderer>();
       
            border.startColor = Color.black;
            border.endColor = Color.black; 
            border.startWidth = 0.02f;
            border.endWidth = 0.02f;
            border.positionCount = 4;
            border.useWorldSpace = false;
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }
        {
            //road 
            var borderObj = new GameObject("Line2");
            borderObj.name = "Line2";
            borderObj.transform.position = new Vector3(0, 0, -1f);
            var border = borderObj.AddComponent<LineRenderer>();
            border.material = new Material(Shader.Find("Sprites/Default"));
            border.startColor = Color.grey;
            border.endColor = Color.grey; 
            border.startWidth = 1f;
            border.endWidth = 1f;
            border.positionCount = 4;
            border.useWorldSpace = false;
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }

    }

    // A function to get the normal point from a point (p) to a line segment (a-b)
    // This function could be optimized to make fewer new Vector objects
    //the intersection between the line and vector p
    Vector3 GetNormalPoint(Vector3 p, Vector3 a, Vector3 b)
    {
        // Vector from a to p
        Vector3 ap = p - a;
        // Vector from a to b
        Vector3 ab = b - a;
        ab.Normalize(); // Normalize the line
        // Project vector "diff" onto line by using the dot product
        ab *= Vector3.Dot(ap, ab);
        Vector3 normalPoint = a + ab;
        return normalPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            debug = !debug;
            for (var i = 0; i < 2; i++)
            {
                var tri = GameObject.Find("Trace" + i);
                tri.GetComponent<Renderer>().enabled = debug;
                var cir = GameObject.Find("Circle" + i);
                cir.GetComponent<Renderer>().enabled = debug;
                var cir2 = GameObject.Find("CircleT" + i);
                cir2.GetComponent<Renderer>().enabled = debug;
                var bli = GameObject.Find("Bling" + i);
                bli.GetComponent<Renderer>().enabled = debug;
            }
        }


        for (var k = 0; k < 2; k++)
        {
            // Predict location 25 (arbitrary choice) frames ahead
            var predict = velocity[k];
            predict.Normalize();
            var pred = predict;
            predict *= 0.8f;
            var predictLoc = location[k] + predict;//predicted location

            // Now we must find the normal to the path from the predicted location
            // We look at the normal for each line segment and pick out the closest one

            Vector3 normal = new Vector3();
            Vector3 target = new Vector3();
            float worldRecord = 1000000;  // Start with a very high record distance that can easily be beaten

            // Loop through all points of the path
            for (int i = 0; i < path.Count - 1; i++)
            {
                // Look at a line segment
              
                var a = path[i];
                var b = path[i + 1];

                // Get the normal point to that line
                var normalPoint = GetNormalPoint(predictLoc, a, b);//predictLoc's vertical line on the ab line
                // This only works because we know our path goes from left to right
                // We could have a more sophisticated test to tell if the point is in the line segment or not
                //IF intersection NOT IN THE LINE AB ,THEN B POINT            
                if (normalPoint.x < a.x || normalPoint.x > b.x)
                {
                    // This is something of a hacky solution, but if it's not within the line segment
                    // consider the normal to just be the end of the line segment (point b)
                    normalPoint = b;
                }

                // How far away are we from the path?
                float distance = Vector3.Distance(predictLoc, normalPoint);

                // Did we beat the record and find the closest line segment?
                if (distance < worldRecord)//SHORTEST DISTANCE
                {
                    worldRecord = distance;
                    // If so the target we want to steer towards is the normal
                    normal = normalPoint;

                    // Look at the direction of the line segment so we can seek a little bit ahead of the normal
                    Vector3 dir = b - a;
                    dir.Normalize();
                    // This is an oversimplification
                    // Should be based on distance to path & velocity
                    dir *= 2f;
                    target = normalPoint;
                    target += dir;//CALCULATE THE TARGET
                }
            }

            GameObject.Find("Circle" + k).transform.position = new Vector3(normal.x, normal.y, -2.5f);
            var bli = normal + pred * 0.4f;
            GameObject.Find("Bling" + k).transform.position = new Vector3(bli.x, bli.y, -2.5f);

            var lr = GameObject.Find("Trace" + k).GetComponent<LineRenderer>();
            var head = location[k] + pred * 0.4f;
            GameObject.Find("CircleT" + k).transform.position = new Vector3(predictLoc.x, predictLoc.y, -2.5f);
            lr.SetPosition(0, new Vector3(head.x, head.y, -2.5f));
            lr.SetPosition(1, new Vector3(predictLoc.x, predictLoc.y, -2.5f));
            lr.SetPosition(2, new Vector3(normal.x, normal.y, -2.5f));

            GameObject.Find("Triangle" + k).transform.position = new Vector3(location[k].x, location[k].y, -2.0f);
            float rot = Vector3.Angle(velocity[k], Vector3.up);
           
            GameObject.Find("Triangle" + k).transform.rotation = Quaternion.AngleAxis(velocity[k].x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//计算三角形的旋转角度
            //Debug.Log(velocity  +" "+Vector3.Angle(velocity, Vector3.right));



            // Only if the distance is greater than the path's radius do we bother to steer
            var sp = GameObject.Find("Bling" + k).GetComponent<SpriteRenderer>();
            if (worldRecord > 0.5f)
            {
                Seek(k, target);
            }

            sp.color = worldRecord > 0.45f ? Color.red : Color.black;

            velocity[k] += acceleration[k];//CALCULATE THE SPEED
            velocity[k] = Vector3.ClampMagnitude(velocity[k], maxspeed[k]);//set the top
            location[k] += velocity[k];
            // Reset accelerationelertion to 0 each cycle
            acceleration[k] *= 0;

            //reset the border
            if (predictLoc.x < left) location[k] = new Vector3(right, 0, location[k].z);
            //if (location.y < -r) location.y = height+r;
            if (predictLoc.x > right) location[k] = new Vector3(left, 0, location[k].z);
        }
    }

    // A method that calculates and applies a steering force towards a target
    // STEER = DESIRED MINUS VELOCITY
    void Seek(int k, Vector3 target)
    {
        var desired = target - location[k];  // A vector pointing from the location to the target

        // If the magnitude of desired equals 0, skip out of here
        // (We could optimize this to check if x and y are 0 to avoid mag() square root
        if (desired.magnitude < float.Epsilon) return;//arrive the target stop moving

        // Normalize desired and scale to maximum speed
        desired.Normalize();
        desired *= maxspeed[k];
        // Steering = Desired minus Velocity
        Vector3 steer = desired - velocity[k];
        steer = Vector3.ClampMagnitude(steer, maxforce[k]);  // Limit to maximum steering force
        acceleration[k] += steer;
    }

    void OnMouseDown()
    {
        path[0] = (new Vector3(left, 0));
        path[1] = (new Vector3(Random.value * left, Random.value * 2.0f - 1f));
        path[2] = (new Vector3(Random.value * right, Random.value * 2.0f - 1f));
        path[3] = (new Vector3(right, 0));

      
        {
            var borderObj = GameObject.Find("Line");
            var border = borderObj.GetComponent<LineRenderer>();
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }
        {
            var borderObj = GameObject.Find("Line2");
            var border = borderObj.GetComponent<LineRenderer>();
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }
    }
}
