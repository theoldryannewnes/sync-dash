using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActions;

public class PlayerController : MonoBehaviour
{

    private const float syncPositionInterval = 0.1f;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private InputActionReference playerActions;
    private bool _isPlaying;
    private bool _isPlayerGrounded;
    private Rigidbody rb;
    private InputAction jumpAction;
    private CanvasController canvasController;
    private GameManager gameManager;

    private float distanceTravelled;
    private int orbsCollected;

    public CanvasController Canvas { set { canvasController = value; } }
    public GameManager Manager { set { gameManager = value; } }

    void Awake()
    {
        _isPlaying = true;
        _isPlayerGrounded = false;
        rb = GetComponent<Rigidbody>();
        if (playerActions != null)
        {
            jumpAction = playerActions.action;
        }
        else
        {
            Debug.LogError("Please assign Jump Action Reference!");
        }
    }

    void Start()
    {
        StartCoroutine(StartDistanceCounterRoutine());
        StartCoroutine(SendPositionRoutine());
    }

    void OnEnable()
    {
        if (jumpAction != null)
        {
            jumpAction.Enable();
            jumpAction.performed += OnJumpPerformed;
        }
    }

    void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJumpPerformed;
            jumpAction.Disable();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isPlaying)
        {
            // Ground check
            if (collision.gameObject.CompareTag("Ground"))
            {
                //Allow next jump
                _isPlayerGrounded = true;
            }

            // Game Over
            if (collision.gameObject.CompareTag("Bump"))
            {
                _isPlaying = false;

                // Enque PlayerAction
                PlayerActions endGameAction = new PlayerActions { Type = PlayerAction.Bump, Timestamp = Time.time };
                NetworkManager.playerActionQueue.Enqueue(endGameAction);

                canvasController.P1GameOver();
                gameManager.GameFinished();
            }

            // PowerUp Collected
            if (collision.gameObject.CompareTag("PowerUp"))
            {
                orbsCollected++;
                // Enque PlayerAction
                PlayerActions endGameAction = new PlayerActions { Type = PlayerAction.Orb, Timestamp = Time.time };
                NetworkManager.playerActionQueue.Enqueue(endGameAction);

                canvasController.P1SetOrbs(orbsCollected);
                gameManager.BreakLastOrb();
            }
        }
    }

    private IEnumerator StartDistanceCounterRoutine()
    {
        distanceTravelled = 0f;
        orbsCollected = 0;

        while (_isPlaying)
        {
            // Wait 1 sec
            yield return new WaitForSeconds(1f);

            // Calculate distance
            distanceTravelled += NetworkManager.MoveSpeed;

            // Set UI value
            canvasController.P1SetScore(distanceTravelled);
        }
    }

    private IEnumerator SendPositionRoutine()
    {
        while (_isPlaying)
        {
            // Enque PlayerPosition
            var posUpdate = new PlayerActions
            {
                Type = PlayerActions.PlayerAction.PlayerPosition,
                Timestamp = Time.time,
                Position = transform.position.y
            };
            NetworkManager.playerActionQueue.Enqueue(posUpdate);

            // Repeat after
            yield return new WaitForSeconds(syncPositionInterval);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_isPlaying && _isPlayerGrounded)
        {
            // Enque PlayerAction
            PlayerActions jumpAction = new PlayerActions { Type = PlayerAction.Jump, Timestamp = Time.time };
            NetworkManager.playerActionQueue.Enqueue(jumpAction);

            // Do physics
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            _isPlayerGrounded = false;

            print("Jump animation");
        }
    }

}
