using UnityEngine;
using UnityEngine.Pool;

public class PowerUpController : MonoBehaviour
{

    [SerializeField] private ParticleSystem ps;
    private Rigidbody rb;
    private Collider sphereCollider;
    private MeshRenderer meshRenderer;
    private bool isActiveInPool;
    private ObjectPool<GameObject> powerUpPool;

    public ObjectPool<GameObject> PowerUpPool { set => powerUpPool = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Release if it goes past the destroy point & has not broken
        if (gameObject.transform.position.z < -10 && isActiveInPool)
        {
            ReleaseToPool();
        }
    }

    private void OnEnable()
    {
        isActiveInPool = true;
        sphereCollider.enabled = true;
        meshRenderer.enabled = true;
    }

    private void ReleaseToPool()
    {
        //Stop movement
        SetVelocity(0f);

        //Release back to Pool
        powerUpPool.Release(gameObject);
    }

    public void SetYOffset(float y)
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + y, gameObject.transform.position.z);
    }

    public void SetVelocity(float speed)
    {
        //Set velocity
        rb.linearVelocity = new Vector3(0f, 0f, speed);
        rb.angularVelocity = Vector3.zero;
    }

    public void BreakOrb()
    {
        //Set velocity to 0
        SetVelocity(0);

        //Disable collider
        sphereCollider.enabled = false;

        //disable mesh
        meshRenderer.enabled = false;

        //Play PS
        ps.Play();

        Invoke("ReleaseToPool", 2f);
    }

}
