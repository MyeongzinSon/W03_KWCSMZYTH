using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public void Move(float speed)
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
}
