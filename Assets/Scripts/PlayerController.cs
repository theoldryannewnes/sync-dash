using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActions;

public class PlayerController : MonoBehaviour
{

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
            canvasController.P1GameOver();
            gameManager.GameFinished();
        }

        // PowerUp Collected
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            orbsCollected++;
            canvasController.P1SetOrbs(orbsCollected);
            collision.gameObject.GetComponent<PowerUpController>()?.BreakOrb();
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
        }
    }

}
