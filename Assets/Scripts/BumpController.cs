using UnityEngine;

public class BumpController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = -1000f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // This value will increase as the game progresses [5-100]
        rb.maxLinearVelocity = GameManager.maxBumpSpeed;
    }


    void FixedUpdate()
    {
        rb.AddForce(new Vector3(0f, 0f, moveSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Bump collided with {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.IncreaseMaxSpeed();
            Destroy(gameObject);
        }
    }

}
