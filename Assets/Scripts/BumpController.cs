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
            ReleaseToPool();
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

        ReleaseToPool();
    }

    private void ReleaseToPool()
    {
        //Stop movement
        SetVelocity(0f);

        //Release back to Pool
        bumpPool.Release(gameObject);
    }

    public void SetVelocity(float speed)
    {
        //Set velocity
        rb.linearVelocity = new Vector3(0f, 0f, speed);
        rb.angularVelocity = Vector3.zero;
    }

    public void DissolveBump()
    {
        StartCoroutine(DissolveBumpRoutine());
    }

}
