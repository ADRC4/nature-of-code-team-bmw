﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line3 : MonoBehaviour {

    int width;
    int height;
    Vector3 mouse;
    Vector3 center;
    float m;
    Vector3 length;


    void Start()
    {
        width = Camera.main.pixelWidth;
        height = Camera.main.pixelHeight;
        Camera.main.transform.Translate(new Vector3(0, 0, 20.0f));

    }

    void Update()
    {


    }

    void OnDrawGizmos()
    {


        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float m = mouse.magnitude;
        length = new Vector3(3, m);
        Vector2 center = Camera.main.ScreenToWorldPoint(new Vector3(width * 0.5f, height * 0.5f));
        mouse = mouse - center;
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(center, 0.05f);
        Gizmos.DrawLine(new Vector2(0, 0), mouse);
      
        


        Gizmos.DrawCube(center, length);


    }


    void OnGUI()
    {
    }
}
