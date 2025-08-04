using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BumpController : MonoBehaviour
{

    [SerializeField] private float dissolveDuration = 3f;

    private Rigidbody rb;
    private float dissolveStrength;
    private ObjectPool<GameObject> bumpPool;

    public ObjectPool<GameObject> BumpPool { set => bumpPool = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Release if it goes past the destroy point
        if (gameObject.transform.position.z < -10)
        {
            //Stop movement
            SetVelocity(0f, true);

            //Release back to Pool
            bumpPool.Release(gameObject);
        }
    }

    private IEnumerator DissolveBumpRoutine()
    {
        float elapsedTime = 0f;
        Material dissolveMat = GetComponent<Renderer>().material;

        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;

            dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            dissolveMat.SetFloat("_DissolveStrength", dissolveStrength);

            yield return null;
        }

        Destroy(gameObject);
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

    public void DissolveBump()
    {
        StartCoroutine(DissolveBumpRoutine());
    }

}
