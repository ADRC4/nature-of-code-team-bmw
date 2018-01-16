using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrack02b : MonoBehaviour {

    List<Vector3> history = new List<Vector3>();
    bool showCircle = false;
    Vector3 location = new Vector3(0, 0, -5.0f);
    Vector3 velocity = new Vector3();
    Vector3 acceleration = new Vector3();
    float maxforce = 0.1f;    // Maximum steering force
    float maxspeed = 4;    // Maximum speed
    GameObject lineObject;
    Vector3 mouse = new Vector3();
    bool running = false;


    // Use this for initialization
    void Start()
    {

        //assets里的edge是用来给三角形和圆形画外轮廓的，但是实践中发现不行，暂且留着，以后改

        GameObject.Find("Circle").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Triangle").GetComponent<Renderer>().enabled = false;
        //lineObject = new GameObject("Line");
        //lineObject.name = "Line";
        //lineObject.AddComponent<LineRenderer>();//画线用
    }

    // Update is called once per frame
    void Update()
    {

        if (!running)
            return;

        // Update velocity
        velocity += acceleration;//计算速度
        velocity = Vector3.ClampMagnitude(velocity, maxspeed);//设置上限
        location += velocity;
        // Reset accelerationelertion to 0 each cycle
        acceleration *= 0;

        history.Add(new Vector3(location.x, location.y, -5.0f));//增加轨迹节点，-5是z坐标，越大越在底层
        if (history.Count > 100)
        {
            history.RemoveAt(0);//去掉最老的线
        }

        //if (history.Count > 1)
        //{
        //    var line = lineObject.GetComponent<LineRenderer>();
        //    //只有设置了材质 setColor才有作用
        //    line.material = new Material(Shader.Find("Particles/Additive"));
        //    line.positionCount = history.Count;//设置两点
        //    line.startColor = Color.black;//这里线不是黑色的，要再研究下
        //    line.endColor = Color.grey; //设置直线颜色
        //    line.startWidth = 0.01f;
        //    line.endWidth = 0.01f;//设置直线宽度

        //    //设置指示线的起点和终点
        //    for (int i = 0; i < history.Count; i++)
        //    {
        //        line.SetPosition(i, history[i]);//轨迹节点
        //    }
        //}
        GameObject.Find("Circle").transform.position = new Vector3(mouse.x, mouse.y, -1.0f);//圆位置
        GameObject.Find("Triangle").transform.position = new Vector3(location.x, location.y, -5.0f);//三角位置
        float rot = Vector3.Angle(velocity, Vector3.up);//旋转角度
        //注意，velocity在第一、四象限时，角度计算不是以顺时针算的，是算最小角度，因此要2pi-rot
        GameObject.Find("Triangle").transform.rotation = Quaternion.AngleAxis(velocity.x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//计算三角形的旋转角度
        //Debug.Log(velocity  +" "+Vector3.Angle(velocity, Vector3.right));
    }

    void OnMouseUp()
    {
        running = false;//停止运动
    }

    // 鼠标拖动
    void OnMouseDrag()
    {
        running = true;
        if (!showCircle)//显示圆形
        {
            showCircle = true;
            GameObject.Find("Circle").GetComponent<Renderer>().enabled = true;
            GameObject.Find("Triangle").GetComponent<Renderer>().enabled = true;//显示出来
        }

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);//获取鼠标所在位置（屏幕坐标，范围0-你的分辨率）
        //要进行转换ScreenToWorldPoint将屏幕坐标转换为世界坐标，世界坐标的范围是x和y在-10到10，z在0到-10(摄影机坐标)
        mouse.z = 0;
        //三角形到鼠标距离
        Vector3 desired = mouse - location;  // A vector pointing from the location to the target
        float d = desired.magnitude;
        // Normalize desired and scale with arbitrary damping within 100 pixels
        desired.Normalize();
        if (d < 100)
        {
            float m = d / 100.0f * maxspeed;
            desired *= m;
        }
        else
        {
            desired *= maxspeed;
        }

        // Steering = Desired minus Velocity
        Vector3 steer = desired - velocity;
        steer = Vector3.ClampMagnitude(steer, maxforce);  // Limit to maximum steering force
        acceleration += steer;
    }
}
