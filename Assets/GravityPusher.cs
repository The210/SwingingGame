using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPusher : MonoBehaviour
{
    public float strength;
    public float range;

    private GameObject player;
    private Rigidbody2D playerRb;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < range)
        {
            Push();
        }
    }

    private void Push()
    {
        float pushForce = Math.Abs((range - Vector3.Distance(transform.position, player.transform.position)) * strength);
        Vector3 pushDirection = (player.transform.position - transform.position).normalized;
        playerRb.AddForce(pushDirection * pushForce);
    }
}
