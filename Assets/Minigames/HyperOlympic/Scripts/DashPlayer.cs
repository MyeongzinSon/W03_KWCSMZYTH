using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Minigames.HyperOlympic.Scripts
{
    public class DashPlayer : MonoBehaviour, IInputListener
    {
        #region Components
        public Animator Anim { get; private set; }
        public GameObject startLine;
        
        #endregion

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
            Anim = transform.GetComponent<Animator>();
            
            //winText.text = "";
            //timerText.text = "Press Space to start!";
            currentSpeed = baseSpeed;

            _isStart = false;
            _isIntro = false;
            _isPlaying = false;
            _isRecording = false;

            _isOnStartLine = false;

            _introCoroutine = IntroCoroutine();
            _moveToStartLineCoroutine = MoveToStartLine();
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
                StartCoroutine(_introCoroutine);
                _isIntro = true;
            }
            if (_isIntro)
            {
                //TODO: On your mark, Get set, Go! + 1s
            }

            if (_isRecording)
            {
            }

            if (_isPlaying)
            {
                CheckUpdateB();
            }
            
            #endregion

            /*if (Input.GetKeyDown(KeyCode.Space) && !isRunning)
            {
                isRunning = true;
                startTime = Time.time;
                lastButtonPressTime = Time.time;
            }
            if (!isRunning)
            {
                isRunning = true;
                startTime = Time.time;
                lastButtonPressTime = Time.time;
            }

            if (isRunning)
            {
                if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastButtonPressTime > buttonPressInterval)
                {
                    buttonPressCount++;
                    lastButtonPressTime = Time.time;
                    currentSpeed += 0.1f;
                }

                float elapsedTime = Time.time - startTime;
                //timerText.text = "Time: " + elapsedTime.ToString("F2") + "s";

                currentSpeed *= speedDecayFactor;
                float distance = elapsedTime * currentSpeed;
                transform.position = new Vector3(distance, transform.position.y, transform.position.z);

                if (distance >= 100.0f)
                {
                    isRunning = false;
                    //winText.text = "You won!";
                }
            }*/
        }

        #region InputListner

        public void UpdateUp()
        {
        }

        public void UpdateDown()
        {
        }

        public void UpdateRight()
        {
        }

        public void UpdateLeft()
        {
        }

        public void UpdateA()
        {
        }

        void CheckUpdateA()
        {
        }

        public void UpdateB()
        {
            if (!_wasInputB)
            {
                _wasInputBTurnedThisFrame = true;
            }

            _isInputB = true;
        }

        void CheckUpdateB()
        {
            if (_isInputB && _wasInputBTurnedThisFrame)
            {
                _wasInputB = true;
                //Debug.Log($"B KeyDown");
                // Run
                buttonPressCount++;
            }

            if (_wasInputB && !_isInputB)
            {
                _wasInputB = false;
                //Debug.Log($"B KeyUp");
            }

            _isInputB = false;
            _wasInputBTurnedThisFrame = false;
        }

        #endregion

        #region DirectInput

        public void OnBInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                buttonPressCount++;
            }
        }

        #endregion
        

        private IEnumerator IntroCoroutine()
        {
            Debug.Log("Intro BGM");
            // Sound: Intro BGM
            yield return new WaitForSeconds(2.0f);
            Debug.Log("On Your Mark");
            // Sound: 'On Your Mark'
            yield return new WaitForSeconds(1f);
            Anim.SetTrigger(GoToMark);
            StartCoroutine(_moveToStartLineCoroutine);
            while (!_isOnStartLine)
            {
                yield return null;
            }
            Anim.SetTrigger(OnMark);
            yield return new WaitForSeconds(0.5f);
            Anim.SetTrigger(OnYourMark);
            yield return new WaitForSeconds(1f);
            Debug.Log("Get Set");
            // Sound: 'Get Set'
            yield return new WaitForSeconds(0.5f);
            Anim.SetTrigger(GetSet);
            yield return new WaitForSeconds(1f);
            Debug.Log("Go!");
            // Sound: 'BANG!!!'
            _isIntro = false;
            
            yield break;
        }
        
        private IEnumerator MoveToStartLine()
        {
            while (Vector3.Distance(transform.position, startLine.transform.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startLine.transform.position, Time.deltaTime);
                yield return null;
            }
            _isOnStartLine = true;
        }
    }
}
