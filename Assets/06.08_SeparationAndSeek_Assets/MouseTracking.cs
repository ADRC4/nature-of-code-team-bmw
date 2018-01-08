using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{

    List<Vector3> location = new List<Vector3> ();
    List<Vector3> acceleration = new List<Vector3> ();
    List<Vector3> velocity = new List<Vector3> ();
    List<GameObject> objects = new List<GameObject> ();
    float maxforce = 0.1f;
    // Maximum steering force
    float maxspeed = 1;
    // Maximum speed
    float r = 6f;
    GameObject circle;
    Vector3 mouse = new Vector3 ();

    // Use this for initialization
    void Start ()
    {
        circle = GameObject.Find ("Circle");
        circle.GetComponent<Renderer> ().enabled = false;//hide 

        for (var i = 0; i < 100; i++) {//create 100 circles
            var newobj = Instantiate (circle);
            newobj.name = "Circle#" + (i + 1);
            newobj.GetComponent<Renderer> ().enabled = true;//show the circles
            objects.Add (newobj);//add circles
            var x = Random.Range (-5, 5);
            var y = Random.Range (-5, 5);//random position
            newobj.transform.position = Vector3.zero;//reset position
            location.Add (new Vector3 (x, y, 0));
            acceleration.Add (Vector3.zero);
            velocity.Add (Vector3.zero);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        
        mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);//get mouse's position
        mouse *= 10;
        mouse.z = 0;

        for (var i = 0; i < objects.Count; i++) {
            var separateForce = separate (i);//force from circles
            var seekForce = seek (i, mouse);//force from mouse
            separateForce *= 2;
            seekForce *= 1;
            acceleration [i] += separateForce;//calculate the acceleration
            acceleration [i] += seekForce;

            // Update velocity
            velocity [i] += acceleration [i];//calculate the velocity
            velocity [i] = Vector3.ClampMagnitude (velocity [i], maxspeed);//set a top
            location [i] += velocity [i];//renew the position
            // Reset accelerationelertion to 0 each cycle
            acceleration [i] *= 0;//reset the acceleration
            objects [i].transform.position = new Vector3 (location [i].x / 10, location [i].y / 10, -5);
        }
    }

    // A method that calculates a steering force towards a target
    // STEER = DESIRED MINUS VELOCITY
    Vector3 seek (int k, Vector3 target)
    {
        var desired = target - location [k];  // A vector pointing from the location to the target

        // Normalize desired and scale to maximum speed
        desired.Normalize ();
        desired *= maxspeed;
        // Steering = Desired minus velocity
        var steer = desired - velocity [k];
      
        steer = Vector3.ClampMagnitude (steer, maxforce);  // Limit to maximum steering force

        return steer;
    }

    // Separation
    // Method checks for nearby vehicles and steers away
    Vector3 separate (int k)
    {
        float desiredseparation = r * 2;
        Vector3 sum = new Vector3 ();
        int count = 0;
        // For every boid in the system, check if it's too close
        foreach (var loc in location) {
            float d = Vector3.Distance (location [k], loc);
            // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
            if ((d > 0) && (d < desiredseparation)) {
                // Calculate vector pointing away from neighbor
                var diff = location [k] - loc;
                diff.Normalize ();
                diff /= d;        // Weight by distance
                sum += diff;      //the force
                count++;          // Keep track of how many
            }
        }
        // Average -- divide by how many
        if (count > 0) {
            sum /= count;//计算合力
            // Our desired vector is the average scaled to maximum speed
            sum.Normalize ();
            sum *= maxspeed;
            //https://cloud.tencent.com/community/article/244115
            // Implement Reynolds: Steering = Desired - Velocity
            sum -= velocity [k];
            sum = Vector3.ClampMagnitude (sum, maxforce);
        }
        return sum;
    }

   
    void OnMouseDrag ()
    {
        mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        mouse *= 10;
        mouse.z = 0;
        //distance from vehicle to target
        var newobj = Instantiate (circle);
        newobj.name = "Circle#" + (objects.Count + 1);
        newobj.GetComponent<Renderer> ().enabled = true;
        objects.Add (newobj);
        newobj.transform.position = Vector3.zero;
        location.Add (new Vector3 (mouse.x, mouse.y, 0));
        acceleration.Add (Vector3.zero);
        velocity.Add (Vector3.zero);
    }
}
