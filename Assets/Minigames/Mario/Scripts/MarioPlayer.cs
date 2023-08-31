using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Minigames.Mario.Scripts
{
    public class MarioPlayer : MonoBehaviour, IInputListener
    {
        #region InputQueue

        private InputQueueRecorder _inputRecorder;
        private InputQueueDecoder _inputDecoder;

        #endregion
        
        #region Components
        
        public CharacterJump characterJump;
        public CharacterMovement characterMovement;
        public Animator Anim { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        
        #endregion
        
        [Header("Inputs")]
        private bool _isInputU;
        private bool _wasInputU;
        private bool _wasInputUTurnedThisFrame;
        private bool _isInputD;
        private bool _wasInputD;
        private bool _wasInputDTurnedThisFrame;
        private bool _isInputR;
        private bool _wasInputR;
        private bool _wasInputRTurnedThisFrame;
        private bool _isInputL;
        private bool _wasInputL;
        private bool _wasInputLTurnedThisFrame;
        private bool _isInputA;
        private bool _wasInputA;
        private bool _wasInputATurnedThisFrame;
        private bool _isInputB;
        private bool _wasInputB;
        private bool _wasInputBTurnedThisFrame;

        private bool _isPressedR;
        private bool _isPressedL;
        
        [Header("States")]
        private bool _isStart;
        private bool _isIntro;
        private bool _isPractice;
        private bool _isRecording;
        private bool _isPlaying;

        [Header("Games")]
        [SerializeField] private bool isClear;

        private Vector3 _startPosition;

        [SerializeField] private GameObject startBoundary;
        
        public bool useBufferedInput;
        public float introTime;
        public float gameTime;
        
        private bool _hasPlayedIntro = false;
        private float _introStartTime;
        
        private bool _isGaming;
        private float _gameStartTime;
        private bool _hasStartedGame = false;

        private Vector2 _lastSpeed;
        private Vector3 _lastPosition;
        private float _originGravityScale;

        private bool _canMove;
        private bool _toggleContinue;
        
        
        [Header("Coroutines")]
        private IEnumerator _introCoroutine;

        #region AnimHash
        private static readonly int GoToMark = Animator.StringToHash("GoToMark");
        private static readonly int OnMark = Animator.StringToHash("OnMark");
        private static readonly int OnYourMark = Animator.StringToHash("OnYourMark");
        private static readonly int GetSet = Animator.StringToHash("GetSet");
        #endregion
        

        private void Awake()
        {
            _inputRecorder = FindObjectOfType<InputQueueRecorder>();
            _inputDecoder = FindObjectOfType<InputQueueDecoder>();
        }

        void Start()
        {
            characterJump = GetComponent<CharacterJump>();
            characterMovement = GetComponent<CharacterMovement>();
            Anim = transform.GetComponent<Animator>();
            Rb = transform.GetComponent<Rigidbody2D>();

            _startPosition = transform.position;
            _originGravityScale = Rb.gravityScale;

            _introStartTime = Time.time;
            _gameStartTime = Time.time;

            _isStart = false;
            _isIntro = false;
            _isPlaying = false;
            _isRecording = false;

            _hasPlayedIntro = false;
            _hasStartedGame = false;

            _canMove = true;
            characterJump.canMove = _canMove;

            isClear = false;
            
            startBoundary.SetActive(true);

            OnSelectGame();
        }

        void Update()
        {
            if (!_hasPlayedIntro && Time.time > _introStartTime + introTime)
            {
                EndIntro();
            }
            else if (useBufferedInput && Time.time > _introStartTime + introTime + gameTime && !_hasStartedGame)
            {
                if (TryDecode())
                {
                    StartGame();
                }
            }

            if (_isGaming || !_hasPlayedIntro)
            {
                //Debug.Log("C1");
                CheckUpdateRight();
                CheckUpdateLeft();
                CheckUpdateA();
                
                // 조기 종료 체크
                
                if (Time.time > _gameStartTime + gameTime)
                {
                    //Debug.Log("C2");
                    OnPlayerDied();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnSelectGame();
            }
        }

        private void FixedUpdate()
        {
            if (_toggleContinue)
            {
                ToggleWorld(_canMove);
                _toggleContinue = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("ClearGame"))
            {
                OnPlayerClear();
            }

            if (other.gameObject.CompareTag("OverGame"))
            {
                OnPlayerDied();
            }
        }

        public void OnSelectGame()
        {
            StartIntro();
        }

        public void ToggleWorld(bool isOn)
        {
            if (isOn)
            {
                // 공간에 정지
                _lastSpeed = Rb.velocity;
                _lastPosition = transform.position;
                Rb.gravityScale = 0;
                Rb.velocity = Vector2.zero;
                transform.position = _lastPosition;
            }
            else
            {
                // 공간에 정지 재개
                Rb.gravityScale = _originGravityScale;
                Rb.velocity = _lastSpeed;
            }
        }

        #region Game States
        
        
        // 연습모드 시작
        void StartIntro()
        {
            _introStartTime = Time.time;
            movementLimiter.instance.CharacterCanMove = true;
            useBufferedInput = false;
            startBoundary.SetActive(true);
        }
        void EndIntro()
        {
            _hasPlayedIntro = true;
            movementLimiter.instance.CharacterCanMove = false;
            useBufferedInput = true;

            _canMove = false;
            characterJump.canMove = _canMove;
            _toggleContinue = true;
            
            OnRecord();
            startBoundary.SetActive(false);
        }
        
        // 녹화된 입력을 재생
        void StartGame()
        {
            _canMove = true;
            characterJump.canMove = _canMove;
            _toggleContinue = true;
            
            _gameStartTime = Time.time;
            _isGaming = true;
            movementLimiter.instance.CharacterCanMove = true;
            _hasStartedGame = true;
        }
        void EndGame()
        {
            _isGaming = false;
            movementLimiter.instance.CharacterCanMove = false;
            Debug.Log("CLEAR? : " + isClear);
            
            //ResetGame();
        }

        void ResetGame()
        {
            transform.position = _startPosition;
            
            _isStart = false;
            _isIntro = false;
            _isPlaying = false;
            _isRecording = false;

            isClear = false;
            startBoundary.SetActive(true);
        }

        #endregion


        public void OnPlayerClear()
        {
            isClear = true;
            EndGame();
        }
        
        public void OnPlayerDied()
        {
            _hasPlayedIntro = true;
            EndGame();
            Debug.Log($"Player died...");
        }

        public void OnRecord()
        {
            if (!useBufferedInput) { return; }
            _inputRecorder.StartRecord(gameTime);
        }
        public bool TryDecode()
        {
            if (!useBufferedInput) { return false; }
        
            var queue = _inputRecorder.GetInputQueues();
        
            if (queue == null) { return false; }

            var time = _inputRecorder.RecordTime;
            _inputDecoder.DecodeInputQueue(queue, time);
            _inputDecoder.StartDecode(this);

            return true;
        }

        #region InputListner

        public void UpdateUp() { }
        public void UpdateDown() { }

        public void UpdateRight()
        {
            characterMovement.directionX = 1;
        }
        void CheckUpdateRight()
        {
            if (_isInputR && _wasInputRTurnedThisFrame)
            {
                _wasInputR = true;
                //Debug.Log($"L KeyDown");
                characterMovement.directionX = 1;
            }

            if (_wasInputR && !_isInputR)
            {
                _wasInputR = false;
                //Debug.Log($"L KeyUp");
                characterMovement.directionX = 0;
            }
            _isInputR = false;
            _wasInputRTurnedThisFrame = false;
        }

        public void UpdateLeft()
        {
            characterMovement.directionX = -1;
        }
        void CheckUpdateLeft()
        {
            if (_isInputL && _wasInputLTurnedThisFrame)
            {
                _wasInputL = true;
                //Debug.Log($"L KeyDown");
                characterMovement.directionX = -1;
            }

            if (_wasInputL && !_isInputL)
            {
                _wasInputL = false;
                //Debug.Log($"L KeyUp");
                characterMovement.directionX = 0;
            }
            _isInputL = false;
            _wasInputLTurnedThisFrame = false;
        }

        public void UpdateA()
        {
            if (!_wasInputA)
            {
                _wasInputATurnedThisFrame = true;
            }
            _isInputA = true;
        }

        void CheckUpdateA()
        {
            if (_isInputA && _wasInputATurnedThisFrame)
            {
                _wasInputA = true;
                //Debug.Log($"A KeyDown");
                characterJump.desiredJump = true;
                characterJump.pressingJump = true;
            }

            if (_wasInputA && !_isInputA)
            {
                _wasInputA = false;
                //Debug.Log($"A KeyUp");
                characterJump.pressingJump = false;
            }
            _isInputA = false;
            _wasInputATurnedThisFrame = false;
        }
        public void UpdateB() { }
        void CheckUpdateB() { }

        #endregion

        #region DirectInput
        
        public void OnUpInput(InputAction.CallbackContext context) { if (useBufferedInput) return; }
        public void OnDownInput(InputAction.CallbackContext context) { if (useBufferedInput) return; }

        public void OnRightInput(InputAction.CallbackContext context)
        {
            if (useBufferedInput) return;
            
            //This function is called when one of the right buttons is pressed.
            
            if (movementLimiter.instance.CharacterCanMove)
            {
                //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
                //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
                if (context.started)
                {
                    _isPressedR = true;
                    characterMovement.directionX = 1f;
                }

                if (context.performed)
                {
                    characterMovement.directionX = 1f;
                }

                if (context.canceled)
                {
                    _isPressedR = false;
                    if (!_isPressedL)
                    {
                        characterMovement.directionX = 0;
                    }
                }
            }
        }
        
        public void OnLeftInput(InputAction.CallbackContext context)
        {
            if (useBufferedInput) return;
            
            //This function is called when one of the left buttons is pressed.
            
            if (movementLimiter.instance.CharacterCanMove)
            {
                //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
                //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
                if (context.started)
                {
                    _isPressedL = true;
                    characterMovement.directionX = -1f;
                }
                
                if (context.performed)
                {
                    characterMovement.directionX = -1f;
                }

                if (context.canceled)
                {
                    _isPressedL = false;
                    if (!_isPressedR)
                    {
                        characterMovement.directionX = 0;
                    }
                }
            }
        }
        
        public void OnAInput(InputAction.CallbackContext context)
        {
            if (useBufferedInput) return;
            
            //This function is called when one of the jump buttons (like space or the A button) is pressed.

            if (movementLimiter.instance.CharacterCanMove)
            {
                //When we press the jump button, tell the script that we desire a jump.
                //Also, use the started and canceled contexts to know if we're currently holding the button
                if (context.started)
                {
                    characterJump.desiredJump = true;
                    characterJump.pressingJump = true;
                }

                if (context.canceled)
                {
                    characterJump.pressingJump = false;
                }
            }
        }
        
        public void OnBInput(InputAction.CallbackContext context)
        {
            if (useBufferedInput) return;
            
            // SpeedMultiplier?
        }

        #endregion
    }
}
