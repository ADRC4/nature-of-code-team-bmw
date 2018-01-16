using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrack08 : MonoBehaviour {

    List<Vector3> location = new List<Vector3>();
    List<Vector3> acceleration = new List<Vector3>();
    List<Vector3> velocity = new List<Vector3>();
    List<GameObject> objects = new List<GameObject>();
    float maxforce = 0.1f;
    // Maximum steering force
    float maxspeed = 1;
    // Maximum speed
    float r = 6f;
    GameObject circle;
    Vector3 mouse = new Vector3();

    // Use this for initialization
    void Start()
    {
        circle = GameObject.Find("Circle");
        circle.GetComponent<Renderer>().enabled = false;//隐藏掉模板

        for (var i = 0; i < 100; i++)
        {//一开始创建100个圆
            var newobj = Instantiate(circle);
            newobj.name = "Circle#" + (i + 1);
            newobj.GetComponent<Renderer>().enabled = true;//显示它
            objects.Add(newobj);//添加圆形
            var x = Random.Range(-5, 5);
            var y = Random.Range(-5, 5);//随机坐标
            newobj.transform.position = Vector3.zero;//位置重置
            location.Add(new Vector3(x, y, 0));
            acceleration.Add(Vector3.zero);
            velocity.Add(Vector3.zero);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //更新取得鼠标的最新位置，由于坐标系不同，这里加10倍，也可以用Camera.ScreenToWorldPoint转换
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);//获取鼠标所在位置（屏幕坐标，范围0-你的分辨率）
        mouse *= 10;
        //要进行转换ScreenToWorldPoint将屏幕坐标转换为世界坐标，世界坐标的范围是x和y在-10到10，z在0到-10(摄影机坐标)
        mouse.z = 0;

        for (var i = 0; i < objects.Count; i++)
        {//对所有粒子进行物理仿真
            var separateForce = separate(i);//得到其他粒子对本粒子的力
            var seekForce = seek(i, mouse);//得到鼠标对本粒子的力
            separateForce *= 2;
            seekForce *= 1;
            acceleration[i] += separateForce;//计算加速度
            acceleration[i] += seekForce;

            // Update velocity
            velocity[i] += acceleration[i];//计算速度
            velocity[i] = Vector3.ClampMagnitude(velocity[i], maxspeed);//设置上限
            location[i] += velocity[i];//更新位置
            // Reset accelerationelertion to 0 each cycle
            acceleration[i] *= 0;//加速度重置
            objects[i].transform.position = new Vector3(location[i].x / 10, location[i].y / 10, -5);
        }
    }

    // A method that calculates a steering force towards a target
    // STEER = DESIRED MINUS VELOCITY
    Vector3 seek(int k, Vector3 target)
    {
        var desired = target - location[k];  // A vector pointing from the location to the target

        // Normalize desired and scale to maximum speed
        desired.Normalize();//当前粒子到鼠标的方向向量
        desired *= maxspeed;
        // Steering = Desired minus velocity
        var steer = desired - velocity[k];//一个力
        //不超上限
        steer = Vector3.ClampMagnitude(steer, maxforce);  // Limit to maximum steering force

        return steer;
    }

    // Separation
    // Method checks for nearby vehicles and steers away
    Vector3 separate(int k)
    {
        float desiredseparation = r * 2;
        Vector3 sum = new Vector3();
        int count = 0;
        // For every boid in the system, check if it's too close
        foreach (var loc in location)
        {
            float d = Vector3.Distance(location[k], loc);//计算距离
            // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
            if ((d > 0) && (d < desiredseparation))
            {//在限定范围内
                // Calculate vector pointing away from neighbor
                var diff = location[k] - loc;//得到某最近粒子到本粒子的方向向量
                diff.Normalize();
                diff /= d;        // Weight by distance
                sum += diff;//计算合力
                count++;            // Keep track of how many
            }
        }
        // Average -- divide by how many
        if (count > 0)
        {
            sum /= count;//计算合力
            // Our desired vector is the average scaled to maximum speed
            sum.Normalize();
            sum *= maxspeed;
            //https://cloud.tencent.com/community/article/244115
            // Implement Reynolds: Steering = Desired - Velocity
            sum -= velocity[k];//变为转向力，使动作平滑
            sum = Vector3.ClampMagnitude(sum, maxforce);
        }
        return sum;
    }

    // 鼠标拖动
    void OnMouseDrag()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);//获取鼠标所在位置（屏幕坐标，范围0-你的分辨率）
        mouse *= 10;
        //要进行转换ScreenToWorldPoint将屏幕坐标转换为世界坐标，世界坐标的范围是x和y在-10到10，z在0到-10(摄影机坐标)
        mouse.z = 0;
        //三角形到鼠标距离
        var newobj = Instantiate(circle);//新建一个圆
        newobj.name = "Circle#" + (objects.Count + 1);
        newobj.GetComponent<Renderer>().enabled = true;
        objects.Add(newobj);//添加圆形
        newobj.transform.position = Vector3.zero;//位置重置
        location.Add(new Vector3(mouse.x, mouse.y, 0));
        acceleration.Add(Vector3.zero);
        velocity.Add(Vector3.zero);
    }
}
