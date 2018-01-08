using System;
using Assets;

namespace Example1_9
{
    public class Mover
    {
        PVector location;
        PVector velocity;
        PVector acceleration;
        float topspeed;

        public Mover()
        {
            location = new PVector(Helper.random(Helper.width), Helper.random(Helper.height));
            velocity = new PVector(Helper.random(-2, 2),  Helper.random(-2, 2));
            acceleration = PVector.random2D();
            acceleration.mult(Helper.random(2));
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
            Helper.fill(175);
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
}

