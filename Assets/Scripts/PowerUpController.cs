using UnityEngine;
using UnityEngine.Pool;

public class PowerUpController : MonoBehaviour
{
    private Rigidbody rb;
    private ObjectPool<GameObject> powerUpPool;

    public ObjectPool<GameObject> PowerUpPool { set => powerUpPool = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Release bump if it goes past the destroy point
        if (gameObject.transform.position.z < -10)
        {
            //Stop movement
            SetVelocity(0f, true);

            //Release back to Pool
            powerUpPool.Release(gameObject);
        }
    }

    public void SetVelocity(float speed, bool stop = false)
    {
        if (stop)
        {
            //Stop Moving
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            //Set velocity
            rb.linearVelocity = new Vector3(0f, 0f, speed);
            rb.angularVelocity = Vector3.zero;
        }
    }

}
