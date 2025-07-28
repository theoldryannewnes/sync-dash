using UnityEngine;

public class BumpController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = -1000f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // This value will increase as the game progresses
        rb.maxLinearVelocity = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector3(x: 0f, y: 0f, z: moveSpeed * Time.deltaTime));
    }

}
