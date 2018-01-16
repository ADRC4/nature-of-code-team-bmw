using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public float xspeed = 10;
    public float yspeed = 3.3f;
    int width;
    int height;
    float centersX;
    float centersY;
    Texture2D image;


    void Start()
    {
        width = Camera.main.pixelWidth;
        height = Camera.main.pixelHeight;
        image = new Texture2D(width, height);


        centersX = width * 0.1f;
        centersY = height * 0.1f;
    }



    void Update()
    {

        PaintCircle();
    }

    void PaintCircle()
    {
        int X = (int)centersX;
        int Y = (int)centersY;


        if ((X > width) || (X < 0))
        {

            xspeed = xspeed * -1;
        }
        if ((Y > height) || (Y < 0))
        {
            yspeed = yspeed * -1;
        }

        centersX += xspeed * 15;
        centersY += yspeed * 15;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                float distance = 9999999;
                float dx = Mathf.Abs(centersX - x);
                float dy = Mathf.Abs(centersY - y);

                float tempDistance = Mathf.Sqrt(dx * dx + dy * dy);

                if (tempDistance < distance) distance = tempDistance;
                Color color = distance < width * 0.08f ? Color.black : Color.white;

                image.SetPixel(x, y, color);
            }
        }

        image.Apply();
    }


    void OnGUI()
    {
        var rectangle = new Rect(0, 0, width, height);
        GUI.DrawTexture(rectangle, image);
    }
}