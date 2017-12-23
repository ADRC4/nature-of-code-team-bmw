using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour
{
    public float speed = 0.0005f;

    void Start()
    {
        // Initial Velocity
        GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
    }
}
