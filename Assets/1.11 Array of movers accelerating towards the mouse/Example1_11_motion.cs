using System.Collections;
using System.Collections.Generic;
using Example1_11;
using Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Example1_11_motion : MonoBehaviour {

    Mover[] movers = new Mover[20];

    void Start()
    {
        Helper.size(200, 200);

        Camera.main.transform.Translate(new Vector3(0, 0, 20.0f));
        Camera.main.transform.Rotate(Vector3.right * 180.0f);

        for (int i = 0; i < movers.Length; i++)
        {
            movers[i] = new Mover();
        }
    }

    void Update()
    {
    }

    void OnDrawGizmos()
    {
        if (movers[0] == null)
            return;

        Helper.background(255);

        for (int i = 0; i < movers.Length; i++)
        {
            movers[i].update();
            movers[i].checkEdges();
            movers[i].display();
        }
    }


   
}
