using UnityEngine;
using UnityEngine.Pool;

public class PowerUpController : MonoBehaviour
{

    [SerializeField] private ParticleSystem ps;
    private Rigidbody rb;
    private Collider sphereCollider;
    private MeshRenderer meshRenderer;
    private GameManager gameManager;
    private ObjectPool<GameObject> powerUpPool;

    public ObjectPool<GameObject> PowerUpPool { set => powerUpPool = value; }
    public GameManager GameManager { set => gameManager = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
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

    private void OnEnable()
    {
        sphereCollider.enabled = true;
        meshRenderer.enabled = true;
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

    public void BreakOrb()
    {
        //Add Points
        gameManager.AddOrbPoint();

        //Disable collider
        sphereCollider.enabled = false;

        //disable mesh
        meshRenderer.enabled = false;

        //Play PS
        ps.Play();
    }

}
