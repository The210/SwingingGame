using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour
{
    public float gravityScaleChange;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        print("enters collider");
        if (collision.tag == "Player")
        {
            print("enters collider as player");
            collision.gameObject.GetComponent<Rigidbody2D>().gravityScale += gravityScaleChange;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().gravityScale -= gravityScaleChange;
        }
    }
}
