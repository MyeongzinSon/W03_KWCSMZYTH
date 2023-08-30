using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    FlappyBirdManager manager;

    public float gravity;
    public float upwardGravityMultiplier;
    public float forcePerSecond;
    public float impulseForce;

    float speedY;
    public bool canMove;

    public void Initialize(FlappyBirdManager manager)
    {
        this.manager = manager;
        speedY = gravity / 2;
    }


    void Update()
    {
        if (!canMove) { return; }
        
        transform.Translate(Vector2.up * speedY * Time.deltaTime);

        if (transform.position.y < -5)
        {
            transform.Translate(Vector2.up * 10);
        }
        else if (transform.position.y > 5)
        {
            transform.Translate(Vector2.down * 10);
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) { return; }

        speedY -= gravity * Time.fixedDeltaTime * (speedY > 0 ? upwardGravityMultiplier : 1);
    }

    public void OnUpdateAddForce()
    {
        if (!canMove) { return; }

        speedY += forcePerSecond * Time.deltaTime * (speedY < 0 ? upwardGravityMultiplier : 1);
    }

    public void AddImpulse()
    {
        if (!canMove) { return; }

        speedY += impulseForce * (speedY < 0 ? upwardGravityMultiplier : 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        manager.OnPlayerDied();
    }

}
