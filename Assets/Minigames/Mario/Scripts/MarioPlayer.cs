using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Minigames.Mario.Scripts
{
    public class MarioPlayer : MonoBehaviour, IInputListener
    {
        #region Components
        
        public CharacterJump characterJump;
        public CharacterMovement characterMovement;
        public Animator Anim { get; private set; }
        
        #endregion
        
        public bool isDirectInput = false;
        [Header("Inputs")]
        private bool _isInputA;
        private bool _wasInputA;
        private bool _wasInputATurnedThisFrame;
        private bool _isInputB;
        private bool _wasInputB;
        private bool _wasInputBTurnedThisFrame;
        [Header("States")]
        private bool _isStart;
        private bool _isIntro;
        private bool _isPractice;
        private bool _isRecording;
        private bool _isPlaying;
        [Header("Games")]
        public float baseSpeed = 1.0f;
        //public Text timerText;
        //public Text winText;
        public float buttonPressInterval = 0.1f;
        public float speedDecayFactor = 0.99f;
        private bool _isOnStartLine;
        [Header("Coroutines")]
        private IEnumerator _introCoroutine;
        private IEnumerator _moveToStartLineCoroutine;

        private float startTime;
        private bool isRunning = false;
        private int buttonPressCount = 10;
        private float lastButtonPressTime;
        private float currentSpeed;

        #region AnimHash
        private static readonly int GoToMark = Animator.StringToHash("GoToMark");
        private static readonly int OnMark = Animator.StringToHash("OnMark");
        private static readonly int OnYourMark = Animator.StringToHash("OnYourMark");
        private static readonly int GetSet = Animator.StringToHash("GetSet");
        #endregion
        

        private void Awake()
        {
        }

        void Start()
        {
            characterJump = GetComponent<CharacterJump>();
            characterMovement = GetComponent<CharacterMovement>();
            Anim = transform.GetComponent<Animator>();
            
            //winText.text = "";
            //timerText.text = "Press Space to start!";
            currentSpeed = baseSpeed;

            _isStart = false;
            _isIntro = false;
            _isPlaying = false;
            _isRecording = false;

            _isOnStartLine = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _isStart = true;
            }
            
            #region Game States

            if (_isStart)
            {
                _isStart = false;
                _isIntro = true;
            }
            if (_isIntro)
            {
                
            }
            if (_isPractice)
            {
                
            }
            if (_isRecording)
            {
                
            }
            if (_isPlaying)
            {
                CheckUpdateB();
            }
            
            #endregion
        }

        #region InputListner

        public void UpdateUp() { }
        public void UpdateDown() { }
        public void UpdateRight() { }
        public void UpdateLeft() { }
        public void UpdateA() { }
        void CheckUpdateA() { }
        public void UpdateB() { }
        void CheckUpdateB() { }

        #endregion

        #region DirectInput
        
        public void OnUpInput(InputAction.CallbackContext context) { if (!isDirectInput) return; }
        public void OnDownInput(InputAction.CallbackContext context) { if (!isDirectInput) return; }

        public void OnLeftInput(InputAction.CallbackContext context)
        {
            if (!isDirectInput) return;
            
            //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
            //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
            if (movementLimiter.instance.CharacterCanMove) {
                characterMovement.directionX = -1 * context.ReadValue<float>();
            }
        }

        public void OnRightInput(InputAction.CallbackContext context)
        {
            if (!isDirectInput) return;
            
            //This is called when you input a direction on a valid input type, such as arrow keys or analogue stick
            //The value will read -1 when pressing left, 0 when idle, and 1 when pressing right.
            if (movementLimiter.instance.CharacterCanMove) {
                characterMovement.directionX = context.ReadValue<float>();
            }
        }
        
        public void OnAInput(InputAction.CallbackContext context)
        {
            if (!isDirectInput) return;
            
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
            if (!isDirectInput) return;
            
            // SpeedMultiplier?
        }

        #endregion
    }
}
