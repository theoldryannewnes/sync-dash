using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;

public class GhostController : MonoBehaviour
{

    [SerializeField] private float jumpForce = 10f;
    private bool _isPlaying;
    private bool _isPlayerGrounded;
    private Rigidbody rb;
    private CanvasController canvasController;
    private GameManager gameManager;

    private float distanceTravelled;
    private int orbsCollected;

    public CanvasController Canvas { set { canvasController = value; } }
    public GameManager Manager { set { gameManager = value; } }

    void Awake()
    {
        _isPlayerGrounded = false;
        _isPlaying = true;
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(StartDistanceCounterRoutine());
    }

    void Update()
    {
        if (NetworkManager.playerActionQueue.Count > 0)
        {
            PlayerActions action = NetworkManager.playerActionQueue.Dequeue();

            if (action.Type == PlayerActions.PlayerAction.Jump)
            {
                DoJump();
            }
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
            canvasController.P2GameOver();
            gameManager.GameFinished();
        }

        // PowerUp Collected
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            orbsCollected++;
            canvasController.P2SetOrbs(orbsCollected);
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
            distanceTravelled += gameManager.currentMoveSpeed;

            // Set UI value
            canvasController.P2SetScore(distanceTravelled);
        }
    }

    private void DoJump()
    {
        if (_isPlaying && _isPlayerGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }
    }

}
