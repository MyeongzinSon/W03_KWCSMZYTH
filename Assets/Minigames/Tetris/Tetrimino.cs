using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum TetriminoType { I, O, T, S, Z, J, L }
public class Tetrimino : MonoBehaviour
{
    public TetriminoType type;
    public Vector3 spawnOffset;

    Rigidbody2D rb;
    TetrisManager gameManager;

    public float scrollSpeed = 1;
    public float manualSpeed = 0.5f;
    public float jumpForce;
    public bool isFocused = true;
    public int rotationType = 0;

    bool isCollidedWithWall = false;
    float wallCollisionDetectValue = 0.99f;
    float wallCheckPeriod = 0.5f;
    float snapMuliplier = 100f;
    float xSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<TetrisManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocused)
        {
            ControlThis();
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = Vector2.right * xSpeed + Vector2.up * rb.velocity.y;
    }

    void ControlThis()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Rotate(true);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Rotate(false);
        }
        // temp code for test
        if (Input.GetButtonDown("Fire3"))
        {
            EndControl();
        }

        xSpeed = scrollSpeed + manualSpeed * Input.GetAxis("Horizontal");
        if (Input.GetAxis("Horizontal") == 0)
        {
            transform.position = Vector3.right * Mathf.Round(transform.position.x * snapMuliplier) / snapMuliplier + Vector3.up * Mathf.Round(transform.position.y * snapMuliplier) / snapMuliplier;
        }
    }
    bool Rotate(bool isClockwise)
    {
        var rotationInfo = TetrisGrid.Instance.CheckRotate(this, isClockwise);
        if (rotationInfo.direction == 0)
        {
            return false;
        }
        else
        {
            rotationType = (rotationType + (isClockwise ? 1 : -1) + 4) % 4;
            transform.Translate(Vector2.up * rotationInfo.aOffset + Vector2.left * rotationInfo.bOffset, Space.World);
            transform.Rotate(Vector3.forward * 90 * rotationInfo.direction);
            return true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if (!TetrisGrid.Instance.CheckFall(this))
        {
            //Debug.Log("It's Wall!");
            isCollidedWithWall = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            //Debug.Log("Can go further");
            isCollidedWithWall = false;
        }
    }
    public List<Transform> GetBlocks()
    {
        var result = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            result.Add(transform.GetChild(i));
        }
        return result;
    }
    void EndControl()
    {
        gameManager.EndControl(this);
    }

}
