using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private bool _isPlaying;
    private bool _isPlayerGrounded;
    private Rigidbody rb;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private InputActionReference playerActions;
    private InputAction jumpAction;

    public bool IsPlaying => _isPlaying;

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
        }

        // PowerUp Collected
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            Debug.Log("Power Up Collected");
            Destroy(collision.gameObject);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_isPlaying && _isPlayerGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            _isPlayerGrounded = false;
        }
    }

}
