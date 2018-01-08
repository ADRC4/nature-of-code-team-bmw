using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{
    static int resolution = 20;
    static int cols = 400 / resolution;
    static int rows = 400 / resolution;
    static float scale = 0.4f;
    Vector3[,] field;
    GameObject[,] fieldObject;

    
    List<Vector3> location = new List<Vector3>();
    List<Vector3> acceleration = new List<Vector3>();
    List<Vector3> velocity = new List<Vector3>();
    List<GameObject> objects = new List<GameObject>();
    //maxforce of vehicle
    List<float> maxforce = new List<float>();
    // Maximum steering force
    //maxspeed of vehicle
    List<float> maxspeed = new List<float>();
    // Maximum speed
    GameObject arrow;//vehicle
    GameObject lineObject;//trail
    bool showLines = true;//show trail

    // Use this for initialization
    void Start()
    {
        arrow = GameObject.Find("Triangle");
        arrow.GetComponent<Renderer>().enabled = false;

        lineObject = new GameObject("Line");
        lineObject.name = "Line";
        var line = lineObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        line.startColor = Color.black;
        line.endColor = Color.black; //trail_color
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;//trail_width
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(0, 0, -1));
        line.SetPosition(1, new Vector3(resolution * scale, 0, -1));

        line.GetComponent<Renderer>().enabled = false;
        line.useWorldSpace = false;
        field = new Vector3[rows, cols];
        fieldObject = new GameObject[rows, cols];//trail_array

        var noisex = Random.value * 100f;//random noise
        var noisey = Random.value * 100f;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var newobj = Instantiate(lineObject);
                newobj.name = "Line#" + i + "_" + j;
                fieldObject[i, j] = newobj;//add trail
                field[i, j] = new Vector3((i - rows / 2) * scale, (j - cols / 2) * scale, 0);
                var loc = new Vector3(field[i, j].x, field[i, j].y, -1);
                float noise = Mathf.PerlinNoise(noisex + i / 5f, noisey + j / 5f);
                field[i, j] = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), 0);
                newobj.transform.position = loc;//reset the position
                var l = newobj.GetComponent<LineRenderer>();
                l.SetPosition(0, new Vector3(0, -resolution * scale * 0.02f, -1));
                l.SetPosition(1, new Vector3(0, resolution * scale * 0.02f, -1));
                float rot = Vector3.Angle(field[i, j], Vector3.up);
                newobj.transform.rotation = Quaternion.AngleAxis(field[i, j].x < 0 ? rot : Mathf.PI*0.5f - rot, Vector3.forward);
            }
        }

        for (var i = 0; i < 120; i++)
        {
            var newobj = Instantiate(arrow);
            newobj.name = "Arrow#" + (i + 1);
            newobj.GetComponent<Renderer>().enabled = true;
            objects.Add(newobj);
            var x = Random.Range(-5, 5);
            var y = Random.Range(-5, 5);//random position
            newobj.transform.position = new Vector3(0, 0, -1);//reset position
            location.Add(new Vector3(Random.Range(-rows / 2, rows / 2) * scale, Random.Range(-cols / 2, cols / 2) * scale, 0));
            acceleration.Add(Vector3.zero);
            velocity.Add(Vector3.zero);
            maxspeed.Add(Random.Range(2, 5) / 20f);
            maxforce.Add(Random.Range(1, 5) / 5f);
        }

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                fieldObject[i, j].GetComponent<Renderer>().enabled = true;
            }
        }
    }

    Vector3 Lookup(Vector3 lookup)
    {
        var row = (int) (Mathf.Clamp(Mathf.FloorToInt((lookup.x - fieldObject[0, 0].transform.position.x) / scale), 0, rows - 1));
        var col = (int) (Mathf.Clamp(Mathf.FloorToInt((lookup.y - fieldObject[0, 0].transform.position.y) / scale), 0, cols - 1));
        return field[row, col];
    }

    void OnMouseDown()
    {
        
        var noisex = Random.value * 100f;
        var noisey = Random.value * 100f;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var newobj = fieldObject[i, j];
                field[i, j] = new Vector3((i - rows / 2) * scale, (j - cols / 2) * scale, 0);
                var loc = new Vector3(field[i, j].x, field[i, j].y, -1);
                float noise = Mathf.PerlinNoise(noisex + i / 5f, noisey + j / 5f);
                field[i, j] = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), 0);
                newobj.transform.position = loc;
                var l = newobj.GetComponent<LineRenderer>();
                l.SetPosition(0, new Vector3(0, -resolution * scale * 0.02f, -1));
                l.SetPosition(1, new Vector3(0, resolution * scale * 0.02f, -1));
                float rot = Vector3.Angle(field[i, j], Vector3.up);
                newobj.transform.rotation = Quaternion.AngleAxis(field[i, j].x < 0 ? rot : Mathf.PI * 0.5f - rot, Vector3.forward);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            showLines = !showLines;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    fieldObject[i, j].GetComponent<Renderer>().enabled = showLines;
                }
            }
        }

        for (var i = 0; i < 120; i++)
        {
            var desired = Lookup(location[i]);
            desired *= maxspeed[i];
            // Steering is desired minus velocity
            var steer = desired - velocity[i];
            steer = Vector3.ClampMagnitude(steer, maxforce[i]);  // Limit to maximum steering force
            acceleration[i] += steer;

            velocity[i] += acceleration[i];
            velocity[i] = Vector3.ClampMagnitude(velocity[i], maxspeed[i]);
            location[i] += velocity[i];
            acceleration[i] *= 0;
            
            float rot = Vector3.Angle(velocity[i], Vector3.up);
          
            objects[i].transform.rotation = Quaternion.AngleAxis(velocity[i].x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//

            var loc = location[i];
            //reset the vehicle when vehicle exceed the border
            if (location[i].x < ( -rows * 0.5f) * scale) loc.x = (rows * 0.5f) * scale;
            if (location[i].y < ( -cols * 0.5f) * scale) loc.y = (cols * 0.5f) * scale;
            if (location[i].x > rows * 0.5f * scale) loc.x = -(rows * 0.5f) * scale;
            if (location[i].y > cols * 0.5f * scale) loc.y = -(cols * 0.5f) * scale;
            location[i] = loc;
            objects[i].transform.position = new Vector3(location[i].x, location[i].y, -5);
        }
    }
}
