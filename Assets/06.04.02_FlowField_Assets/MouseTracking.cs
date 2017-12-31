using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{
    static int resolution = 20;
    static int cols = 400 / resolution;
    static int rows = 400 / resolution;
    static float scale = 0.4f;
    Vector3[,] field;
    GameObject[,] fieldObject;

    //下面都是箭头的位置、速度、加速度
    List<Vector3> location = new List<Vector3>();
    List<Vector3> acceleration = new List<Vector3>();
    List<Vector3> velocity = new List<Vector3>();
    List<GameObject> objects = new List<GameObject>();
    //箭头的最大加速度
    List<float> maxforce = new List<float>();
    // Maximum steering force
    //箭头的最大速度
    List<float> maxspeed = new List<float>();
    // Maximum speed
    GameObject arrow;//箭头原型
    GameObject lineObject;//直线原型
    bool showLines = true;//显示直线

    // Use this for initialization
    void Start()
    {
        arrow = GameObject.Find("Triangle");
        arrow.GetComponent<Renderer>().enabled = false;//隐藏掉模板

        lineObject = new GameObject("Line");//准备直线
        lineObject.name = "Line";
        var line = lineObject.AddComponent<LineRenderer>();//画线用
        line.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));//改成这样的就可以了
        line.startColor = Color.black;//这里线不是黑色的，要再研究下
        line.endColor = Color.black; //设置直线颜色
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;//设置直线宽度
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(0, 0, -1));//这里设置默认，其实没用
        line.SetPosition(1, new Vector3(resolution * scale, 0, -1));

        line.GetComponent<Renderer>().enabled = false;
        line.useWorldSpace = false;
        field = new Vector3[rows, cols];//向量图
        fieldObject = new GameObject[rows, cols];//直线对象数组

        var noisex = Random.value * 100f;//噪声的随机起点
        var noisey = Random.value * 100f;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var newobj = Instantiate(lineObject);
                newobj.name = "Line#" + i + "_" + j;
                fieldObject[i, j] = newobj;//添加线
                field[i, j] = new Vector3((i - rows / 2) * scale, (j - cols / 2) * scale, 0);//设置每个直线的位置
                var loc = new Vector3(field[i, j].x, field[i, j].y, -1);
                float noise = Mathf.PerlinNoise(noisex + i / 5f, noisey + j / 5f);//取噪声值
                field[i, j] = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), 0);
                newobj.transform.position = loc;//位置重置
                var l = newobj.GetComponent<LineRenderer>();
                l.SetPosition(0, new Vector3(0, -resolution * scale * 0.02f, -1));//直线的本地坐标系
                l.SetPosition(1, new Vector3(0, resolution * scale * 0.02f, -1));
                float rot = Vector3.Angle(field[i, j], Vector3.up);//旋转角度
                newobj.transform.rotation = Quaternion.AngleAxis(field[i, j].x < 0 ? rot : Mathf.PI*0.5f - rot, Vector3.forward);//直线旋转角度
            }
        }

        for (var i = 0; i < 120; i++)
        {
            var newobj = Instantiate(arrow);
            newobj.name = "Arrow#" + (i + 1);
            newobj.GetComponent<Renderer>().enabled = true;//显示它
            objects.Add(newobj);//添加圆形
            var x = Random.Range(-5, 5);//箭头的
            var y = Random.Range(-5, 5);//随机坐标
            newobj.transform.position = new Vector3(0, 0, -1);//位置重置
            location.Add(new Vector3(Random.Range(-rows / 2, rows / 2) * scale, Random.Range(-cols / 2, cols / 2) * scale, 0));
            acceleration.Add(Vector3.zero);
            velocity.Add(Vector3.zero);
            maxspeed.Add(Random.Range(2, 5) / 20f);
            maxforce.Add(Random.Range(1, 5) / 5f);
        }

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                fieldObject[i, j].GetComponent<Renderer>().enabled = true;
            }
        }
    }

    //根据箭头当前坐标得到箭头所在位置的向量
    Vector3 Lookup(Vector3 lookup)
    {
        var row = (int) (Mathf.Clamp(Mathf.FloorToInt((lookup.x - fieldObject[0, 0].transform.position.x) / scale), 0, rows - 1));
        var col = (int) (Mathf.Clamp(Mathf.FloorToInt((lookup.y - fieldObject[0, 0].transform.position.y) / scale), 0, cols - 1));
        return field[row, col];
    }

    void OnMouseDown()
    {
        //重置地图
        var noisex = Random.value * 100f;
        var noisey = Random.value * 100f;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var newobj = fieldObject[i, j];
                field[i, j] = new Vector3((i - rows / 2) * scale, (j - cols / 2) * scale, 0);
                var loc = new Vector3(field[i, j].x, field[i, j].y, -1);
                float noise = Mathf.PerlinNoise(noisex + i / 5f, noisey + j / 5f);
                field[i, j] = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), 0);
                newobj.transform.position = loc;//位置重置
                var l = newobj.GetComponent<LineRenderer>();
                l.SetPosition(0, new Vector3(0, -resolution * scale * 0.02f, -1));
                l.SetPosition(1, new Vector3(0, resolution * scale * 0.02f, -1));
                float rot = Vector3.Angle(field[i, j], Vector3.up);//旋转角度
                newobj.transform.rotation = Quaternion.AngleAxis(field[i, j].x < 0 ? rot : Mathf.PI * 0.5f - rot, Vector3.forward);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            showLines = !showLines;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    fieldObject[i, j].GetComponent<Renderer>().enabled = showLines;
                }
            }
        }

        for (var i = 0; i < 120; i++)
        {
            var desired = Lookup(location[i]);//得到箭头所在位置的向量
            desired *= maxspeed[i];
            // Steering is desired minus velocity
            var steer = desired - velocity[i];//平滑过渡，将速度向量设置为偏向过渡
            steer = Vector3.ClampMagnitude(steer, maxforce[i]);  // Limit to maximum steering force
            acceleration[i] += steer;

            velocity[i] += acceleration[i];
            velocity[i] = Vector3.ClampMagnitude(velocity[i], maxspeed[i]);
            location[i] += velocity[i];
            acceleration[i] *= 0;
            
            float rot = Vector3.Angle(velocity[i], Vector3.up);//旋转角度
            //注意，velocity在第一、四象限时，角度计算不是以顺时针算的，是算最小角度，因此要2pi-rot
            objects[i].transform.rotation = Quaternion.AngleAxis(velocity[i].x < 0 ? rot : (2 * Mathf.PI - rot), Vector3.forward);//计算三角形的旋转角度

            var loc = location[i];
            //当箭头的坐标超出地图时，重置它
            if (location[i].x < ( -rows * 0.5f) * scale) loc.x = (rows * 0.5f) * scale;
            if (location[i].y < ( -cols * 0.5f) * scale) loc.y = (cols * 0.5f) * scale;
            if (location[i].x > rows * 0.5f * scale) loc.x = -(rows * 0.5f) * scale;
            if (location[i].y > cols * 0.5f * scale) loc.y = -(cols * 0.5f) * scale;
            location[i] = loc;
            objects[i].transform.position = new Vector3(location[i].x, location[i].y, -5);
        }
    }
}
