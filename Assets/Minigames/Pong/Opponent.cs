using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    PongManager player;
    PongController pongController;
    public float minSpeed;
    public float maxSpeed;
    [Range(0f,1f)] public float offsetRange;

    private float twoBarDistance;
    private float currentSpeed;
    private float targetOffset;

    private void Awake()
    {
        player = FindObjectOfType<PongManager>();
        pongController = FindObjectOfType<PongController>();
        twoBarDistance = player.transform.position.x - transform.position.x;
        InvokeRepeating("RandomOffset", .5f, 1f);

    }

    private void Update()
    {
        UpdateSpeed();
        FollowBall();
    }

    void FollowBall()
    {
        float yDist = pongController.transform.position.y + targetOffset - transform.position.y;
        if (yDist > 0) { Move(1); }
        else { Move(-1); }
    }

    private void Move(float verticalAxis)
    {
        if (verticalAxis != 0)
        {
            Vector2 direction = Vector2.up * verticalAxis;
            var destination = transform.position + (Vector3)direction * currentSpeed * Time.deltaTime;
            transform.position = player.ClampPlayerInBoundaries(destination);
        }
    }

    void UpdateSpeed()
    {
        if (PongManager.isGameRunning)
            currentSpeed = Mathf.Lerp(maxSpeed, minSpeed, (pongController.transform.position.x - transform.position.x) / twoBarDistance);
        else currentSpeed = 0;
    }

    void RandomOffset()
    {
        targetOffset = Random.Range(-transform.lossyScale.x * offsetRange, transform.lossyScale.x * offsetRange);
    }

    //die

}
