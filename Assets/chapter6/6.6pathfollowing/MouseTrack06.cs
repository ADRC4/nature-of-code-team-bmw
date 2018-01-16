using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrack06 : MonoBehaviour {
    static float left = -6.0f; //边框的四个点
    static float right = 6.0f;
    static float up = 4.0f;
    static float down = -4.0f;

    //两部车子的设置
    static float[] maxforce = { 0.002f, 0.012f }; // Maximum steering force
    static float[] maxspeed = { 0.04f, 0.12f };    // Maximum speed
    Vector3[] location = { new Vector3(left, 0, 0), new Vector3(left, 0, 0) };
    Vector3[] velocity = { new Vector3(maxspeed[0], 0), new Vector3(maxspeed[1], 0) };
    Vector3[] acceleration = { new Vector3(), new Vector3() };

    bool debug = true;//调试

    List<Vector3> path = new List<Vector3>();//管道，4个点

    // Use this for initialization
    void Start()
    {
        path.Add(new Vector3(left, 0));//左边中点
        path.Add(new Vector3(Random.value * left, Random.value * 2.0f - 1f));//上下波动范围-1~1
        path.Add(new Vector3(Random.value * right, Random.value * 2.0f - 1f));//上下波动范围-1~1
        path.Add(new Vector3(right, 0));//右边中点
        //assets里的edge是用来给三角形和圆形画外轮廓的，但是实践中发现不行，暂且留着，以后改

        //两个模版，要隐藏掉
        GameObject.Find("Circle").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Triangle").GetComponent<Renderer>().enabled = false;

        {
            for (int i = 0; i < 2; i++)
            {
                //克隆一堆东西，有圆和三角形
                var tri = Instantiate(GameObject.Find("Triangle"));//小车本身
                tri.GetComponent<Renderer>().enabled = true;
                tri.name = "Triangle" + i;
                var cir = Instantiate(GameObject.Find("Circle"));//小车在轨道上的点
                cir.GetComponent<Renderer>().enabled = true;
                cir.name = "Circle" + i;
                cir.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                var cir2 = Instantiate(GameObject.Find("Circle"));//小车前方的点
                cir2.GetComponent<Renderer>().enabled = true;
                cir2.name = "CircleT" + i;
                cir2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                var bling = Instantiate(GameObject.Find("Circle"));//小车在轨道上的较大的点，会闪
                bling.GetComponent<Renderer>().enabled = true;
                bling.name = "Bling" + i;
                bling.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);


                {
                    var borderObj = new GameObject("Trace" + i);//画小车前方的直线
                    borderObj.name = "Trace" + i;
                    borderObj.transform.position = new Vector3(0, 0, -2.4f);
                    var border = borderObj.AddComponent<LineRenderer>();//画线用


                    border.material = new Material(Shader.Find("Sprites/Default"));//改成这样的就可以了
                    border.startColor = Color.red;
                    border.endColor = Color.black; //设置直线颜色
                    border.startWidth = 0.02f;
                    border.endWidth = 0.02f;//设置直线宽度
                    border.positionCount = 3;
                    border.useWorldSpace = true;
                    //border.SetPosition(0, Vector3.zero);//把三个点要弄齐了
                    //border.SetPosition(1, Vector3.zero);
                    //border.SetPosition(2, Vector3.zero);
                }
            }
        }

        //画轨道
        {
            //轨道上的中心线，细
            var borderObj = new GameObject("Line");//准备直线
            borderObj.name = "Line";
            borderObj.transform.position = new Vector3(0, 0, -3f);
            var border = borderObj.AddComponent<LineRenderer>();//画线用
            border.material = new Material(Shader.Find("Sprites/Default"));//试来试去这个才是对的！！
            //Finds a shader with the given name.
            border.startColor = Color.black;//这里线不是黑色的，要再研究下
            border.endColor = Color.black; //设置直线颜色
            border.startWidth = 0.02f;
            border.endWidth = 0.02f;//设置直线宽度
            border.positionCount = 4;
            border.useWorldSpace = false;
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }
        {
            //轨道背景，粗
            var borderObj = new GameObject("Line2");
            borderObj.name = "Line2";
            borderObj.transform.position = new Vector3(0, 0, -1f);
            var border = borderObj.AddComponent<LineRenderer>();//画线用
            border.material = new Material(Shader.Find("Sprites/Default"));//改成这样的就可以了
            border.startColor = Color.grey;
            border.endColor = Color.grey; //设置直线颜色
            border.startWidth = 1f;
            border.endWidth = 1f;//设置直线宽度
            border.positionCount = 4;
            border.useWorldSpace = false;
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }

    }

    // A function to get the normal point from a point (p) to a line segment (a-b)
    // This function could be optimized to make fewer new Vector objects
    //求向量p到线段ab上的交点
    Vector3 GetNormalPoint(Vector3 p, Vector3 a, Vector3 b)
    {
        // Vector from a to p
        Vector3 ap = p - a;
        // Vector from a to b
        Vector3 ab = b - a;
        ab.Normalize(); // Normalize the line
        // Project vector "diff" onto line by using the dot product
        ab *= Vector3.Dot(ap, ab);
        Vector3 normalPoint = a + ab;
        return normalPoint;
    }

    // Update is called once per framep
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            debug = !debug;//是否要隐藏掉
            for (var i = 0; i < 2; i++)
            {
                var tri = GameObject.Find("Trace" + i);
                tri.GetComponent<Renderer>().enabled = debug;
                var cir = GameObject.Find("Circle" + i);
                cir.GetComponent<Renderer>().enabled = debug;
                var cir2 = GameObject.Find("CircleT" + i);
                cir2.GetComponent<Renderer>().enabled = debug;
                var bli = GameObject.Find("Bling" + i);
                bli.GetComponent<Renderer>().enabled = debug;
            }
        }


        for (var k = 0; k < 2; k++)//对两辆小车操作
        {
            // Predict location 25 (arbitrary choice) frames ahead
            var predict = velocity[k];//小车速度向量
            predict.Normalize();
            var pred = predict;
            predict *= 0.8f;//调整速度，向前看
            var predictLoc = location[k] + predict;//过一会小车要达到的位置

            // Now we must find the normal to the path from the predicted location
            // We look at the normal for each line segment and pick out the closest one

            Vector3 normal = new Vector3();//小车的predict点到轨道上的法向量
            Vector3 target = new Vector3();//小车的目标移动位置
            float worldRecord = 1000000;  // Start with a very high record distance that can easily be beaten

            // Loop through all points of the path
            for (int i = 0; i < path.Count - 1; i++)
            {
                // Look at a line segment
                //线段
                var a = path[i];
                var b = path[i + 1];

                // Get the normal point to that line
                var normalPoint = GetNormalPoint(predictLoc, a, b);//predictLoc点在ab线段上的垂线
                // This only works because we know our path goes from left to right
                // We could have a more sophisticated test to tell if the point is in the line segment or not
                //交点不在ab线段上，就默认为b点
                if (normalPoint.x < a.x || normalPoint.x > b.x)
                {
                    // This is something of a hacky solution, but if it's not within the line segment
                    // consider the normal to just be the end of the line segment (point b)
                    normalPoint = b;
                }

                // How far away are we from the path?
                float distance = Vector3.Distance(predictLoc, normalPoint);//算一下从向前看的点到轨道上法向交点的距离

                // Did we beat the record and find the closest line segment?
                if (distance < worldRecord)//用于找最短距离，可能与另一轨道更近呢？
                {
                    worldRecord = distance;
                    // If so the target we want to steer towards is the normal
                    normal = normalPoint;

                    // Look at the direction of the line segment so we can seek a little bit ahead of the normal
                    Vector3 dir = b - a;
                    dir.Normalize();
                    // This is an oversimplification
                    // Should be based on distance to path & velocity
                    dir *= 2f;
                    target = normalPoint;
                    target += dir;//算出target，也就是小车直线要去的地方
                }
            }

            //一些debug图形的位置设置
            GameObject.Find("Circle" + k).transform.position = new Vector3(normal.x, normal.y, -2.5f);
            var bli = normal + pred * 0.4f;
            GameObject.Find("Bling" + k).transform.position = new Vector3(bli.x, bli.y, -2.5f);

            var lr = GameObject.Find("Trace" + k).GetComponent<LineRenderer>();
            var head = location[k] + pred * 0.4f;
            GameObject.Find("CircleT" + k).transform.position = new Vector3(predictLoc.x, predictLoc.y, -2.5f);
            lr.SetPosition(0, new Vector3(head.x, head.y, -2.5f));
            lr.SetPosition(1, new Vector3(predictLoc.x, predictLoc.y, -2.5f));
            lr.SetPosition(2, new Vector3(normal.x, normal.y, -2.5f));

            GameObject.Find("Triangle" + k).transform.position = new Vector3(location[k].x, location[k].y, -2.0f);//三角位置
            float rot = Vector3.Angle(velocity[k], Vector3.up);//旋转角度
            //注意，velocity在第一、四象限时，角度计算不是以顺时针算的，是算最小角度，因此要2pi-rot
            GameObject.Find("Triangle" + k).transform.rotation = Quaternion.AngleAxis(velocity[k].x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//计算三角形的旋转角度
            //Debug.Log(velocity  +" "+Vector3.Angle(velocity, Vector3.right));



            // Only if the distance is greater than the path's radius do we bother to steer
            var sp = GameObject.Find("Bling" + k).GetComponent<SpriteRenderer>();
            if (worldRecord > 0.5f)//什么时候要调整小车的方向？就是小车到轨道外面去的时候，由于轨道粗1.0f，所以这里是一半
            {
                Seek(k, target);
            }

            sp.color = worldRecord > 0.45f ? Color.red : Color.black;//是否是红色

            velocity[k] += acceleration[k];//计算速度
            velocity[k] = Vector3.ClampMagnitude(velocity[k], maxspeed[k]);//设置上限
            location[k] += velocity[k];
            // Reset accelerationelertion to 0 each cycle
            acceleration[k] *= 0;

            //边界重置
            if (predictLoc.x < left) location[k] = new Vector3(right, 0, location[k].z);
            //if (location.y < -r) location.y = height+r;
            if (predictLoc.x > right) location[k] = new Vector3(left, 0, location[k].z);
        }
    }

    // A method that calculates and applies a steering force towards a target
    // STEER = DESIRED MINUS VELOCITY
    void Seek(int k, Vector3 target)//小车要调整方向了！
    {
        var desired = target - location[k];  // A vector pointing from the location to the target

        // If the magnitude of desired equals 0, skip out of here
        // (We could optimize this to check if x and y are 0 to avoid mag() square root
        if (desired.magnitude < float.Epsilon) return;//都到了目标地点，不用走了

        // Normalize desired and scale to maximum speed
        desired.Normalize();
        desired *= maxspeed[k];
        // Steering = Desired minus Velocity
        Vector3 steer = desired - velocity[k];//老样子，拐过来
        steer = Vector3.ClampMagnitude(steer, maxforce[k]);  // Limit to maximum steering force
        acceleration[k] += steer;
    }

    void OnMouseDown()
    {
        path[0] = (new Vector3(left, 0));
        path[1] = (new Vector3(Random.value * left, Random.value * 2.0f - 1f));
        path[2] = (new Vector3(Random.value * right, Random.value * 2.0f - 1f));
        path[3] = (new Vector3(right, 0));

        //画轨道
        {
            var borderObj = GameObject.Find("Line");//准备直线
            var border = borderObj.GetComponent<LineRenderer>();//画线用
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }
        {
            var borderObj = GameObject.Find("Line2");
            var border = borderObj.GetComponent<LineRenderer>();//画线用
            border.SetPosition(0, path[0]);
            border.SetPosition(1, path[1]);
            border.SetPosition(2, path[2]);
            border.SetPosition(3, path[3]);
        }
    }
}
