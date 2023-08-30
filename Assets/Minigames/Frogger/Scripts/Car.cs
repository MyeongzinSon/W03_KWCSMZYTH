using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    bool _hasInitialized = false;
    float _velocity;
    float _dir;
    float _xBound;
    bool _isMovable = false;
    List<Car> _cars;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_hasInitialized && _isMovable) {
            transform.position += new Vector3(_velocity * Time.deltaTime, 0, 0);

            if (_dir < 0 && transform.position.x < _xBound) {
                Destroy(this.gameObject);
                _cars.Remove(this);
            } else if (_dir > 0 && transform.position.x > _xBound) {
                Destroy(this.gameObject);
                _cars.Remove(this);
            }
        }
    }

    public void Init(Vector3 position, float velocity, float dir, float xBound, ref List<Car> cars) {
        transform.position = position;
        _hasInitialized = true;
        _velocity = velocity * Mathf.Sign(dir);
        _dir = dir;
        _xBound = xBound;
        _isMovable = true;

        if (dir < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        _cars = cars;
        cars.Add(this);
    }

    public void Stop() {
        _isMovable = false;
    }

    public void Resume() {
        _isMovable = true;
    }
}
