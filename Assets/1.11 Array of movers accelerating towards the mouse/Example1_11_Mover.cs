using System;
using Assets;

namespace Example1_11
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
            velocity = new PVector(0, 0);
            topspeed = 4;
        }

        public void update()
        {
            PVector mouse = new PVector(Helper.mouseX, Helper.mouseY);
            PVector dir = PVector.sub(mouse, location);

            dir.normalize();
            dir.mult(0.5f);
            acceleration = dir;

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

