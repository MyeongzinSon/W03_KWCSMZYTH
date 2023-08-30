using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject firstPrefab;
    [SerializeField] private GameObject middlePrefab;
    [SerializeField] private GameObject lastPrefab;

    private int _count;
    private Vector2Int _curFirstPos;
    private Vector2 _curFirstWorldPos;
    private int _velocity;
    private Func<Vector2, Vector2Int> _firstPosToTilePos;
    private Func<Vector2Int, Vector2> _toWorldPosFunc;
    private List<GameObject> _platforms = new List<GameObject>();
    private bool _isHaveFrog;
    private int _frogIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Vector2Int firstPos, int count, Func<Vector2Int, Vector2> toWorldPosFunc, Func<Vector2, Vector2Int> toTilePosFunc) {
        _curFirstPos = firstPos;
        _count = count;
        _toWorldPosFunc = toWorldPosFunc;
        _firstPosToTilePos = toTilePosFunc;

        for (int i = 0; i < count; i++) {
            GameObject platform;
            if (i == 0) {
                platform = Instantiate(firstPrefab, toWorldPosFunc(firstPos), Quaternion.identity);
            } else if (i == count - 1) {
                platform = Instantiate(lastPrefab, toWorldPosFunc(firstPos), Quaternion.identity);
            } else {
                platform = Instantiate(middlePrefab, toWorldPosFunc(firstPos), Quaternion.identity);
            }
            _platforms.Add(platform);
        }
    }

    public List<Vector2Int> UpdatePlatform() {
        _curFirstWorldPos += new Vector2(_velocity * Time.deltaTime, 0);
        for (int i = 0; i < _count; i++) {
            _platforms[i].transform.position = _curFirstWorldPos + new Vector2(i, 0);
        }

        if (_firstPosToTilePos(_curFirstWorldPos) != _curFirstPos) {
            _curFirstPos = _firstPosToTilePos(_curFirstWorldPos);
        }

        List<Vector2Int> tilePos = new List<Vector2Int>();
        for (int i = 0; i < _count; i++) {
            tilePos.Add(_curFirstPos + new Vector2Int(i, 0));
        }

        return tilePos;
    }

    public void SetOnFrog() {
        _isHaveFrog = true;   
    }
}
