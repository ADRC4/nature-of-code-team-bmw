using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public static class Helper
    {
        private static Stack<PVector> vectors = new Stack<PVector>();
        private static PVector pt = new PVector();

        public static float mouseX
        {
            get { return Input.mousePosition.x; }
        }

        public static float mouseY
        {
            get { return Input.mousePosition.y; }
        }

        public static int width
        {
            get { return Camera.main.pixelWidth; }
        }

        public static int height
        {
            get { return Camera.main.pixelHeight; }
        }

        public static void background(int i)
        {
            Gizmos.color = new Color(i / 255.0f , i / 255.0f, i / 255.0f);
            Gizmos.DrawCube(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(30.0f, 30.0f, 1.0f));
        }

        public static void fill(int i)
        {
            Gizmos.color = new Color(i / 255.0f, i / 255.0f, i / 255.0f);
        }

        public static void stroke(int i)
        {
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
            Gizmos.DrawSphere((Vector2)Camera.main.ScreenToWorldPoint(new PVector(x + pt.x, y + pt.y)), (r1 + r2) * 0.016f);
        }

        public static void popMatrix()
        {
            pt = vectors.Peek();
            vectors.Pop();
        }
    }
}
