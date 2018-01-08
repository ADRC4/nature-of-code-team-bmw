using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public struct PVector
    {
        public float x { set; get; }
        public float y { set; get; }

        public PVector(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public void add(PVector v)
        {
            x += v.x;
            y += v.y;
        }

        public void sub(PVector v)
        {
            x -= v.x;
            y -= v.y;
        }

        public void set(PVector v)
        {
            x = v.x;
            y = v.y;
        }

        public void mult(float f)
        {
            x *= f;
            y *= f;
        }

        public static PVector sub(PVector v1, PVector v2)
        {
            return new PVector(v1.x - v2.x, v1.y - v2.y);
        }

        public void normalize()
        {
            float magnitude = this.magnitude;
            if ((double) magnitude > 9.99999974737875E-06)
                this.set(this / magnitude);
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }

        public void limit(float f)
        {
            if ((double)this.sqrMagnitude > (double)f * (double)f)
                this.set(this.normalized * f);
        }

        public static float dist(PVector v1, PVector v2)
        {
            return (v1 - v2).magnitude;
        }

        public void div(float f)
        {
            x /= f;
            y /= f;
        }

        public static PVector random2D()
        {
            return UnityEngine.Random.insideUnitCircle;
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

        public static PVector operator +(PVector a, PVector b)
        {
            return new PVector(a.x + b.x, a.y + b.y);
        }

        public static PVector operator -(PVector a, PVector b)
        {
            return new PVector(a.x - b.x, a.y - b.y);
        }

        public static PVector operator -(PVector a)
        {
            return new PVector(-a.x, -a.y);
        }

        public static PVector operator *(PVector a, float d)
        {
            return new PVector(a.x * d, a.y * d);
        }

        public static PVector operator *(float d, PVector a)
        {
            return new PVector(a.x * d, a.y * d);
        }

        public static PVector operator /(PVector a, float d)
        {
            return new PVector(a.x / d, a.y / d);
        }

        public static implicit operator PVector(Vector3 v)
        {
            return new PVector(v.x, v.y);
        }

        public static implicit operator PVector(Vector2 v)
        {
            return new PVector(v.x, v.y);
        }

        public static implicit operator Vector3(PVector v)
        {
            return new Vector3(v.x, v.y, 0.0f);
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

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}
