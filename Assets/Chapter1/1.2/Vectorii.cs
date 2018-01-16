using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectorii: MonoBehaviour
{

    
    int width;
    int height;
    float centersX;
    float centersY;
    Texture2D image;
    Vector3 location;
    Vector3 velocity;
    

   

    void Start()
    {
        width = Camera.main.pixelWidth;
        height = Camera.main.pixelHeight;
        image = new Texture2D(width, height);

        location = new Vector3(centersX, centersY);
        velocity = new Vector3(10,3.3f);

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


        if ((location.x > width) || (location.x < 0))
        {

            velocity.x = velocity.x * -1;
        }
        if ((location.y > height) || (location.y < 0))
        {
            velocity.y = velocity.y * -1;
        }



        location = location + velocity;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                float distance = 9999999;
                float dx = Mathf.Abs(location.x - x);
                float dy = Mathf.Abs(location.y - y);

                float tempDistance = Mathf.Sqrt(dx * dx + dy * dy);

                if (tempDistance < distance) distance = tempDistance;
                Color color = distance < width * 0.08f ? Color.black : Color.white;

                image.SetPixel(x, y, color);
            }
        }

        //g.DrawEllipse(new Pen(Color.Red), int x, int y, int width, int height);

        image.Apply();
    }


    void OnGUI()
    {
        var rectangle = new Rect(0, 0, width, height);
        GUI.DrawTexture(rectangle, image);
    }
}
