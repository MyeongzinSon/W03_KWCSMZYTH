using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTestTest : MonoBehaviour
{
    public GameObject g;
    private GameObject _g;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            _g = Instantiate(g, new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Y)) {
            Destroy(_g);
        }
    }
}
