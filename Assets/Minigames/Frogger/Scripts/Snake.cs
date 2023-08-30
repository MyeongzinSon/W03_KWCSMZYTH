using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Vector3 position, float waitTime, bool isRandom) {
        transform.position = position;
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D.enabled = false;
        _spriteRenderer.enabled = false;

        StartCoroutine(UpdateSnake(waitTime, isRandom));
    }

    IEnumerator UpdateSnake(float waitTime, bool isRandom) {
        yield return new WaitForSeconds(waitTime);

        while (true) {
            _animator.SetTrigger("Apear");
            yield return new WaitForSeconds(0.3f);
            _boxCollider2D.enabled = true;
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(1.3f);
            _boxCollider2D.enabled = false;
            _animator.SetTrigger("Disappear");
            yield return new WaitForSeconds(0.3f);
            _spriteRenderer.enabled = false;
            if (isRandom) {
                yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));
            } else {
                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}
