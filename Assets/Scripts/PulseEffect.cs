using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float timeBtwSpwans;
    public float startTimeBtwSpawns;
    public GameObject echo;
    // Update is called once per frame

    void Update()
    {
            if (timeBtwSpwans <= 0)
            {
                Instantiate(echo, transform.position, Quaternion.identity);
                timeBtwSpwans = startTimeBtwSpawns;
            }
            else
            {
                timeBtwSpwans -= Time.deltaTime;
            }
    }
}
