using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PongController : MonoBehaviour
{
    private Temp_PongPlayer player;
    private Rigidbody2D rb2d;

    private float pongTopY;
    private float pongBottomY;
    private float pongLeftX;
    private float pongRightX;

    private float finalPongSpeed;
    Vector2 lastV;

    private void Awake()
    {
        player = FindObjectOfType<Temp_PongPlayer>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        pongTopY = player.topY - transform.lossyScale.y / 2f;
        pongBottomY = player.bottomY + transform.lossyScale.y / 2f;
        pongLeftX = player.leftX + transform.lossyScale.x / 2f;
        pongRightX = player.rightX - transform.lossyScale.x / 2f;

        finalPongSpeed = player.startingPongSpeed;
        rb2d.velocity = player.startingPongDirection.normalized * finalPongSpeed;
    }

    private void Update()
    {
        if (Temp_PongPlayer.isGameRunning == false)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }
        if (rb2d.velocity == Vector2.zero) { rb2d.velocity = lastV; }
        if (transform.position.x < pongLeftX || transform.position.x > pongRightX)
        {
            
        }
        if (transform.position.y < pongBottomY || transform.position.y > pongTopY)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * -1);
        }
        transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, pongBottomY, pongTopY));

        if (Temp_PongPlayer.isGameRunning) lastV = rb2d.velocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bar")
        {

            float pongDirection = rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x);
            Vector2 barPosition = other.transform.position;
            float reflectRatio = (barPosition.y - transform.position.y) / (other.transform.lossyScale.y / 2f);
            reflectRatio = Mathf.Clamp(reflectRatio, -1f, 1f);
            Debug.Log(reflectRatio);
            float reflectAngle = player.maxAngle * reflectRatio;
            Vector2 result = Quaternion.AngleAxis(reflectAngle, Vector3.forward) * Vector2.left;

            result = new Vector2(result.x * pongDirection, result.y);

            rb2d.velocity = result.normalized * rb2d.velocity.magnitude;

            if (other.GetComponent<Temp_PongPlayer>() != null)
            {
                if(other.GetComponent<Temp_PongPlayer>().isAfterDecode)
                {
                    FindObjectOfType<Opponent>().gameObject.SetActive(false);
                }
            }
        }
    }
}
