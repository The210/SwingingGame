using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{

    public float timeBetweenSpawns;
    public GameObject screenTopLeft;
    public Vector2 screenSize;
    public GameObject coin;
    public GameObject movingCoin;

    private float timeCounter = 0f;

    void Update()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter >= timeBetweenSpawns)
        {
            timeCounter = 0;
            if (Random.Range(0, 10) < 9)
                Instantiate(coin, new Vector3(screenTopLeft.transform.position.x + Random.Range(screenSize.x / 5, screenSize.x), screenTopLeft.transform.position.y - Random.Range(screenSize.y / 5, screenSize.y)), Quaternion.identity);
            else
                Instantiate(movingCoin, new Vector3(screenTopLeft.transform.position.x + Random.Range(screenSize.x / 4, screenSize.x * 0.8f), screenTopLeft.transform.position.y - Random.Range(screenSize.y / 4, screenSize.y * 0.8f)), Quaternion.identity);
        }
    }
}
