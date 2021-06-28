using UnityEngine;

namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline.PlayerCharacter
{
    public class CatMovement : MonoBehaviour
    {
        public CharacterController characterController;
        public float defaultWalkingSpeed = 0.2f;
        public float defaultRunningSpeed = 0.4f;
        public float turnSmoothTime = 0.3f;
        public float acceleration = 3f;
        public float timeUntilSleep = 3;
        public float timeToWakeUp = 2.1f;
        public Animator animator;

        private float turnSmoothVelocity = 0.0f;

        private int isWalkingHash;
        private int isRunningHash;
        private int isSleepingHash;

        private PlayerInput input;

        private Vector2Int CurrentInput { get; set; }
        private Vector3 CurrentInputToVector3 => new Vector3(-CurrentInput.x, 0f, -CurrentInput.y);
        private Vector3 CurrentDirection { get; set; } = Vector3.zero;
        private bool MovementPressed => CurrentInput.magnitude != 0f;
        private bool RunPressed { get; set; }

        private bool Forward = false;
        private bool Left = false;
        private bool Backward = false;
        private bool Right = false;
        private bool Forward1 = false;
        private bool Left1 = false;
        private bool Backward1 = false;
        private bool Right1 = false;
        
        private float TimeSinceLatestMovement { get; set; } = 0f;
        private float TimeSinceLatestSleep { get; set; } = 100 + float.Epsilon;

        private bool IsWalking
        {
            get => animator.GetBool(isWalkingHash);
            set => animator.SetBool(isWalkingHash, value);
        }

        private bool IsRunning
        {
            get => animator.GetBool(isRunningHash);
            set => animator.SetBool(isRunningHash, value);
        }

        void Awake()
        {
            input = new PlayerInput();

            input.CharacterControls.Forward.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Forward = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Left.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Left = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Backward.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Backward = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Right.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Right = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Run.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                RunPressed = ctx.ReadValueAsButton();
            };
            
            input.CharacterControls.Forward1.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Forward1 = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Left1.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Left1 = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Backward1.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Backward1 = ctx.ReadValueAsButton();
            };
            input.CharacterControls.Right1.performed += ctx =>
            {
                ResetTimeSinceLatestMovement();
                Right1 = ctx.ReadValueAsButton();
            };
        }

        void Start()
        {
            isWalkingHash = Animator.StringToHash("IsWalking");
            isRunningHash = Animator.StringToHash("IsRunning");
            isSleepingHash = Animator.StringToHash("IsSleeping");
        }

        void Update()
        {
            if (GameManager.gameHasStarted)
            {
                SetCurrentInput();
                HandleSleep();
                HandleMovement();

                if (IsRunning)
                {
                    GameManager.analyitcsData.timeRunning += Time.deltaTime;
                }
                else if (IsWalking)
                {
                    GameManager.analyitcsData.timeWalking += Time.deltaTime;
                }
            }
        }

        private void SetCurrentInput()
        {
            CurrentInput = Vector2Int.zero;
            if (Forward || Forward1) CurrentInput += Vector2Int.up;
            if (Left || Left1) CurrentInput += Vector2Int.left;
            if (Backward || Backward1) CurrentInput += Vector2Int.down;
            if (Right || Right1) CurrentInput += Vector2Int.right;
        }

        private void HandleSleep()
        {
            if (CurrentInput == Vector2Int.zero)
            {
                TimeSinceLatestMovement += Time.deltaTime;
            }

            if (TimeSinceLatestMovement > timeUntilSleep)
            {
                animator.SetBool(isSleepingHash, true);
                TimeSinceLatestSleep = 0f;
            }

            if (TimeSinceLatestMovement < timeUntilSleep)
            {
                animator.SetBool(isSleepingHash, false);
                TimeSinceLatestSleep += Time.deltaTime;
            }
        }

        void HandleMovement()
        {
            
            bool isRunning = IsRunning;
            bool isWalking = IsWalking;

            if (MovementPressed && !isWalking) IsWalking = true;
            if (!MovementPressed && isWalking) IsWalking = false;

            if (MovementPressed && RunPressed && !isRunning) IsRunning = true;
            if (!(MovementPressed && RunPressed) && isRunning) IsRunning = false;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Standing still")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Walking forward")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("Running forward"))
            {
                float currentAngle = transform.eulerAngles.y;
                float targetAngle = GetTargetAngle();
                float angle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref turnSmoothVelocity,
                    turnSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                float speed = 0f;
                if (isWalking) speed = defaultWalkingSpeed;
                if (isRunning) speed = defaultRunningSpeed;

                CurrentDirection = (CurrentDirection + CurrentInputToVector3 * acceleration * Time.deltaTime)
                    .normalized;

                characterController.Move(CurrentDirection * speed * Time.deltaTime);
            }
            else
            {
                CurrentDirection = Vector3.zero;
            }
        }

        private float GetTargetAngle()
        {
            if (CurrentInput == Vector2.up) return 270;
            if (CurrentInput == Vector2.up + Vector2.right) return 315;
            if (CurrentInput == Vector2.right) return 0;
            if (CurrentInput == Vector2.right + Vector2.down) return 45;
            if (CurrentInput == Vector2.down) return 90;
            if (CurrentInput == Vector2.down + Vector2.left) return 135;
            if (CurrentInput == Vector2.left) return 180;
            if (CurrentInput == Vector2.left + Vector2.up) return 225;

            return transform.eulerAngles.y;
        }

        void OnEnable()
        {
            input.CharacterControls.Enable();
        }

        void OnDisable()
        {
            input.CharacterControls.Disable();
        }

        private void ResetTimeSinceLatestMovement()
        {
            this.TimeSinceLatestMovement = 0f;
        }
    }
}