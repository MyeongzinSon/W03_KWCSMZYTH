using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Frog : MonoBehaviour
{
    public enum MoveType {
        FrontJump,
        BackJump,
        LeftJump,
        RightJump,
        Start,
        Die
    }

    private class FrogParameter {
        public static string AnimTrigger(MoveType type) {
            switch(type) {
                case MoveType.FrontJump: return "FrontJump";
                case MoveType.BackJump: return "BackJump";
                case MoveType.LeftJump:
                case MoveType.RightJump: return "SideJump";
                case MoveType.Start: return "Start";
                case MoveType.Die: return "Die";
                default: return "";
            }
        }

        public static Vector2Int MoveDir(MoveType type) {
            switch(type) {
                case MoveType.FrontJump: return Vector2Int.up;
                case MoveType.BackJump: return Vector2Int.down;
                case MoveType.LeftJump: return Vector2Int.left;
                case MoveType.RightJump: return Vector2Int.right;
                default: return Vector2Int.zero;
            }
        }
    }

    [SerializeField] private float moveTime;
    [SerializeField] private float dieTime;
    
    private Animator _animator;
    private Vector2Int _initPosition;
    private Vector2Int _position;
    private Func<Vector2Int, Vector2> _toWorldPosFunc;
    private Action _onDie;
    private bool _isDie = false;

    void Start() {
        _isDie = false;
        _animator = GetComponent<Animator>();
    }

    void Update() {
        
    }

    public void Init(Vector2Int position, Func<Vector2Int, Vector2> toWorldPosFunc, Action onDie) {
        _animator = GetComponent<Animator>();
        _animator.SetTrigger(FrogParameter.AnimTrigger(MoveType.Start));
        _initPosition = position;
        _toWorldPosFunc = toWorldPosFunc;
        _onDie = onDie;
        _isDie = false;

        SetPosition(_initPosition);
    }

    public void Move(MoveType type) {
        if (!_isDie) {
            StartCoroutine(PlayAnimAndMove(type));
        }
    }

    public void Die() {
        StartCoroutine(FrogDie());
        _onDie();       
    }

    private IEnumerator PlayAnimAndMove(MoveType type) {
        if (type == MoveType.LeftJump) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
        }

        _animator.SetTrigger(FrogParameter.AnimTrigger(type));

        Vector2Int moveDir = FrogParameter.MoveDir(type);
        Vector2Int nextPos = _position + moveDir;

        float startTime = Time.time;
        Vector2 startPos = transform.position;
        while (startTime + moveTime > Time.time) {
            transform.position = Vector2.Lerp(startPos, _toWorldPosFunc(nextPos), (Time.time - startTime) / moveTime);
            yield return null;
        }

        SetPosition(nextPos);
    }

    private IEnumerator FrogDie() {
        _animator.SetTrigger(FrogParameter.AnimTrigger(MoveType.Die));
        yield return new WaitForSeconds(dieTime);
        
        //SetPosition(_initPosition);
    }

    private void SetPosition(Vector2Int position) {
        _position = position;
        transform.position = _toWorldPosFunc(_position);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy") && !_isDie) {
            _isDie = true;
            Debug.Log("Die");
            Die();
        }
    }
}
