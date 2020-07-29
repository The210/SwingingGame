using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float coinLifeSpan;
    private float timeRemaining;
    private SpriteRenderer coinSprite;
    private float ratio;
    void Start()
    {
        ratio = 1 / coinLifeSpan; //used for range to transform time alive to transparency.
        coinSprite = this.GetComponent<SpriteRenderer>();
        timeRemaining = coinLifeSpan;

    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
            Destroy(this.gameObject);
        Color tmp = coinSprite.color;
        tmp.a = timeRemaining * ratio;
        coinSprite.color = tmp;
    }
}
