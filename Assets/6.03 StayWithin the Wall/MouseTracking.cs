using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{

    List<Vector3> history = new List<Vector3>();
    Vector3 location = new Vector3(0, 0, -5.0f);
    Vector3 velocity = new Vector3(0.05f, 0.06f);
    Vector3 acceleration = new Vector3();
    float maxforce = 0.005f;    // Maximum steering force
    float maxspeed = 0.1f;    // Maximum speed
    GameObject lineObject;
    float left = -5.0f;//边框的四个点
    float right = 5.0f;
    float up = 4.0f;
    float down = -4.0f;

    // Use this for initialization
    void Start()
    {

        //assets里的edge是用来给三角形和圆形画外轮廓的，但是实践中发现不行，暂且留着，以后改

        GameObject.Find("Circle").GetComponent<Renderer>().enabled = false;//不要它了
        GameObject.Find("Triangle").GetComponent<Renderer>().enabled = true;
        lineObject = new GameObject("Line");
        lineObject.name = "Line";
        var line = lineObject.AddComponent<LineRenderer>();//画线用
        line.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));//改成这样的就可以了
        line.startColor = Color.black;//这里线不是黑色的，要再研究下
        line.endColor = Color.black; //设置直线颜色
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;//设置直线宽度

        //画边框
        var borderObj = new GameObject("Border");//准备直线
        borderObj.name = "Border";
        var border = borderObj.AddComponent<LineRenderer>();//画线用
        border.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));//改成这样的就可以了
        border.startColor = Color.blue;//这里线不是黑色的，要再研究下
        border.endColor = Color.blue; //设置直线颜色
        border.startWidth = 0.02f;
        border.endWidth = 0.02f;//设置直线宽度
        border.positionCount = 5;
        border.SetPosition(0, new Vector3(left, down, -0.5f));//边框的四个点
        border.SetPosition(1, new Vector3(left, up, -0.5f));
        border.SetPosition(2, new Vector3(right, up, -0.5f));
        border.SetPosition(3, new Vector3(right, down, -0.5f));
        border.SetPosition(4, new Vector3(left, down, -0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desired = new Vector3(-500, -500);

        //碰到边界就法线方向速度分量反转并成速度最大值，切线方向不变
        if (location.x < left)
        {
            desired = new Vector3(maxspeed, velocity.y);
        }
        else if (location.x > right)
        {
            desired = new Vector3(-maxspeed, velocity.y);
        }

        if (location.y < down)
        {
            desired = new Vector3(velocity.x, maxspeed);
        }
        else if (location.y > up)
        {
            desired = new Vector3(velocity.x, -maxspeed);
        }

        if (desired.x > -500)
        {
            desired.Normalize();
            desired *= maxspeed;
            var steer = desired - velocity;//转向过渡
            steer = Vector3.ClampMagnitude(steer, maxforce);
            acceleration += steer;
        }

        // Update velocity
        velocity += acceleration;//计算速度
        velocity = Vector3.ClampMagnitude(velocity, maxspeed);//设置上限
        location += velocity;
        // Reset accelerationelertion to 0 each cycle
        acceleration *= 0;

        history.Add(new Vector3(location.x, location.y, -5.0f));//增加轨迹节点，-5是z坐标，越大越在底层
        if (history.Count > 500)
        {
            history.RemoveAt(0);//去掉最老的线
        }

        if (history.Count > 1)
        {
            var line = lineObject.GetComponent<LineRenderer>();
            //只有设置了材质 setColor才有作用
            line.positionCount = history.Count;//设置两点

            //设置指示线的起点和终点
            for (int i = 0; i < history.Count; i++)
            {
                line.SetPosition(i, history[i]);//轨迹节点
            }
        }

        GameObject.Find("Triangle").transform.position = new Vector3(location.x, location.y, -5.0f);//三角位置
        float rot = Vector3.Angle(velocity, Vector3.up);//旋转角度
        //注意，velocity在第一、四象限时，角度计算不是以顺时针算的，是算最小角度，因此要2pi-rot
        GameObject.Find("Triangle").transform.rotation = Quaternion.AngleAxis(velocity.x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//计算三角形的旋转角度
        //Debug.Log(velocity  +" "+Vector3.Angle(velocity, Vector3.right));
    }
}
