using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCoin : MonoBehaviour
{
    public float coinLifeSpan;
    private float timeRemaining;
    private SpriteRenderer coinSprite;
    private float ratio;
    private Vector3 direction;
    public float minSpeed;
    public float maxSpeed;
    private float speed;
    private float timeSinceChange = 0f;
    public float timePerDirection;
    void Start()
    {
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        direction.Normalize();
        speed = Random.Range(minSpeed, maxSpeed);
        ratio = 1 / coinLifeSpan; //used for range to transform time alive to transparency.
        coinSprite = this.GetComponent<SpriteRenderer>();
        timeRemaining = coinLifeSpan;

    }

    void Update()
    {
        timeSinceChange += Time.deltaTime;
        if (timeSinceChange >= timePerDirection)
        {
            direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            direction.Normalize();
            timeSinceChange = 0;
        }
        print(direction);
        transform.position += direction * speed;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
            Destroy(this.gameObject);
        Color tmp = coinSprite.color;
        tmp.a = timeRemaining * ratio;
        coinSprite.color = tmp;
    }
}
