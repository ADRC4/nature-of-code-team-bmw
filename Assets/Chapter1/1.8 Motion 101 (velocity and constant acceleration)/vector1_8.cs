﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vector1_8 : MonoBehaviour {
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



        mover.update();
        mover.checkEdges();
        mover.display();
    }



    public static class Helper
    {
        private static Stack<PVector> vectors = new Stack<PVector>();
        private static PVector pt = new PVector();



        public static int width
        {
            get { return Camera.main.pixelWidth; }
        }

        public static int height
        {
            get { return Camera.main.pixelHeight; }
        }


        public static float random(float f)
        {
            return UnityEngine.Random.Range(0, f);
        }

        public static float random(float f1, float f2)
        {
            return UnityEngine.Random.Range(f1, f2);
        }

        public static void size(int w, int h)
        {
            Screen.SetResolution(w, h, false);
        }

        public static void pushMatrix()
        {
            vectors.Push(pt);
            pt.x = 0;
            pt.y = 0;
        }

        public static void translate(float locationX, float locationY)
        {
            pt.x += locationX;
            pt.y += locationY;
        }

        public static void ellipse(float x, float y, float r1, float r2)
        {
            Gizmos.DrawSphere((Vector2)Camera.main.ScreenToWorldPoint(new PVector(x + pt.x, y + pt.y)), (r1 + r2) * 0.008f);
        }

        public static void popMatrix()
        {
            pt = vectors.Peek();
            vectors.Pop();
        }
    }


    public class Mover
    {
        PVector location;
        PVector velocity;
        PVector acceleration;

        float topspeed;

        public Mover()
        {
            location = new PVector(Helper.random(Helper.width), Helper.random(Helper.height));
            velocity = new PVector(Helper.random(-2, 2), Helper.random(-2, 2));
            acceleration = new PVector(-0.001f, 0.01f);
            topspeed = 10;
        }

        public void update()
        {
            velocity.add(acceleration);
            velocity.limit(topspeed);
            location.add(velocity);
        }

        public void display()
        {

            Helper.ellipse(location.x, location.y, 16, 16);
        }

        public void checkEdges()
        {
            if (location.x > Helper.width)
            {
                location.x = 0;
            }
            else if (location.x < 0)
            {
                location.x = Helper.width;
            }

            if (location.y > Helper.height)
            {
                location.y = 0;
            }
            else if (location.y < 0)
            {
                location.y = Helper.height;
            }
        }
    }


    public class PVector
    {
        public float x { set; get; }
        public float y { set; get; }

        public PVector()
        {
            this.x = 0;
            this.y = 0;
        }

        public PVector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void add(PVector v)
        {
            x += v.x;
            y += v.y;
        }

        public void limit(float f)
        {
            
       
            if ((double)this.sqrMagnitude > (double)f * (double)f)
                this.set(this.normalized * f);
      

        }
        public void normalize()
        {
            float magnitude = this.magnitude;
            if ((double)magnitude > 9.99999974737875E-06)
                this.set(this / magnitude);
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }

        public PVector normalized
        {
            get
            {
                PVector PVector = new PVector(this.x, this.y);
                PVector.normalize();
                return PVector;
            }
        }

        public void set(PVector v)
        {
            x = v.x;
            y = v.y;
        }

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt((float)((double)this.x * (double)this.x + (double)this.y * (double)this.y));
            }
        }

        public float sqrMagnitude
        {
            get
            {
                return (float)((double)this.x * (double)this.x + (double)this.y * (double)this.y);
            }
        }

        public static PVector operator /(PVector a, float d)
        {
            return new PVector(a.x / d, a.y / d);
        }

        public static PVector operator +(PVector a, PVector b)
        {
            return new PVector(a.x + b.x, a.y + b.y);
        }

        public static PVector operator *(PVector a, float d)
        {
            return new PVector(a.x * d, a.y * d);
        }


        public static implicit operator Vector3(PVector v)
        {
            return new Vector3(v.x, v.y, 0.0f);
        }

    }
}
