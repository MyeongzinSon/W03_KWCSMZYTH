using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prefabSceneLoader : MonoBehaviour
{
    public GameObject prefabScene;
    private GameObject loadedScene;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            loadedScene = Instantiate(prefabScene, new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (loadedScene)
            {
                Destroy(loadedScene);
            }
        }
    }
}
