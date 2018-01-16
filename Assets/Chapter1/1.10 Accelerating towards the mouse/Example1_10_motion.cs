using System.Collections;
using System.Collections.Generic;
using Example1_10;
using Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Example1_10_motion : MonoBehaviour {

    Mover mover;

    void Start()
    {
        Helper.size(200, 200);

        Camera.main.transform.Translate(new Vector3(0, 0, 20.0f));
        Camera.main.transform.Rotate(Vector3.right * 180.0f);

        mover = new Mover();
    }

    void Update()
    {
    }

    void OnDrawGizmos()
    {
        if (mover == null)
            return;

        Helper.background(255);

        mover.update();
        mover.checkEdges();
        mover.display();
    }


    
}
