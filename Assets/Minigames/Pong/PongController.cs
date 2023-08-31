using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PongController : MonoBehaviour
{
    private PongManager player;
    private Opponent opponent;
    private Rigidbody2D rb2d;

    private float pongTopY;
    private float pongBottomY;
    private float pongLeftX;
    private float pongRightX;

    private float finalPongSpeed;
    private int opponentHitCount;
    float alarmTime;
    Vector2 lastV;
    bool isGameFinished;

    private void Awake()
    {
        player = FindObjectOfType<PongManager>();
        opponent = FindObjectOfType<Opponent>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isGameFinished = false;
        pongTopY = player.topY - transform.lossyScale.y / 2f;
        pongBottomY = player.bottomY + transform.lossyScale.y / 2f;
        pongLeftX = opponent.transform.position.x + opponent.transform.lossyScale.x/2f + transform.lossyScale.x / 2f;
        pongRightX = player.rightX - transform.lossyScale.x / 2f;

        finalPongSpeed = player.startingPongSpeed;
        rb2d.velocity = player.startingPongDirection.normalized * finalPongSpeed;
        alarmTime = Mathf.Infinity;
    }

    private void Update()
    {
        alarmTime -= Time.deltaTime;
        if ( alarmTime <= player.alarmInitialDelay) 
        {
            //UI ½ÃÀÛ
            player.SetRecord(alarmTime);
            player.SetDecode(alarmTime + player.recordDuration + .5f);
            alarmTime = Mathf.Infinity;
        }
        if (PongManager.isGameRunning == false)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }
        if (rb2d.velocity == Vector2.zero) { rb2d.velocity = lastV; }
        if (transform.position.x < pongLeftX)
        {
            if (opponent.gameObject.activeSelf == true)
            {
                opponentHitCount++;
                transform.position = new Vector2(pongLeftX, transform.position.y);
                rb2d.velocity = new Vector2(rb2d.velocity.x *-1, rb2d.velocity.y);
                if (player.recordAlarmOpponentHitCount == opponentHitCount)
                {
                    alarmTime = ((pongRightX - transform.position.x) / rb2d.velocity.x) + player.alarmTimeAfterPlayerHit;
                }
            }
            else if (transform.position.x < player.leftX && !isGameFinished) 
            { 
                GameManager.Instance.MiniGameClear(); 
                isGameFinished = true;
            }
        }
        if (transform.position.x > pongRightX && !isGameFinished)
        {
            GameManager.Instance.MiniGameOver();
            isGameFinished = true;
        }
        if (transform.position.y < pongBottomY || transform.position.y > pongTopY)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * -1);
        }
        transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, pongBottomY, pongTopY));

        if (PongManager.isGameRunning) lastV = rb2d.velocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bar")
        {

            float pongDirection = rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x);
            Vector2 barPosition = other.transform.position;
            float reflectRatio = (barPosition.y - transform.position.y) / (other.transform.lossyScale.y / 2f);
            float absRR = Mathf.Abs(reflectRatio);
            absRR = Mathf.Clamp(absRR, .2f, 1f);
            if(reflectRatio != 0)
            {
                reflectRatio = absRR * reflectRatio / Mathf.Abs(reflectRatio);
            }
            reflectRatio = Mathf.Clamp(reflectRatio, -1f, 1f);
            float reflectAngle = player.maxAngle * reflectRatio;
            Vector2 result = Quaternion.AngleAxis(reflectAngle, Vector3.forward) * Vector2.left;

            result = new Vector2(result.x * pongDirection, result.y);

            rb2d.velocity = result.normalized * rb2d.velocity.magnitude;

            if (other.GetComponent<PongManager>() != null)
            {
                if(other.GetComponent<PongManager>().isAfterDecode)
                {
                    FindObjectOfType<Opponent>().gameObject.SetActive(false);
                }
            }
        }
    }
}
