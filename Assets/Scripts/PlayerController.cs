using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private bool _isPlaying;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody rb;
    [SerializeField] private InputActionReference playerActions;
    private InputAction jumpAction;
    private bool _isPlayerGrounded;

    public bool IsPlaying => _isPlaying;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerActions != null)
        {
            jumpAction = playerActions.action;
        }
        else
        {
            Debug.Log("Please assign Jump Action Reference!");
        }
    }

    void Start()
    {
        _isPlaying = true;
        _isPlayerGrounded = false;
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
        //Debug.Log($"Player collided with {collision.gameObject.name}");

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
            Debug.Log("Game Over condition reached!");
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_isPlaying && _isPlayerGrounded)
        {
            //Debug.Log("Jumping..");
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            _isPlayerGrounded = false;
        }
    }

}
