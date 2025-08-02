using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{

    public static float currentMoveSpeed;
    private const float maxSpawnTime = 8f;
    private const float minSpawnTime = 3f;

    [SerializeField] private CanvasController canvasController;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bumpPrefab;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform ghostSpawn;
    [SerializeField] private Transform bumpSpawn;
    [SerializeField] private Transform powerUpSpawn;

    private List<GameObject> bumpObjects = new List<GameObject>();
    private List<GameObject> powerUpObjects = new List<GameObject>();
    private PlayerController playerController;

    private int orbsCollected;
    private float distanceTravelled;
    private bool isShowingEndPanel;

    private IObjectPool<GameObject> bumpPool;
    private bool bumpCollectionCheck = true;
    private int bumpPoolCapacity = 10;
    private int bumpPoolMaxSize = 20;

    private float nextBumpSpawnTime;

    private IObjectPool<GameObject> powerUpPool;
    private bool powerUpCollectionCheck = true;
    private int powerUpPoolCapacity = 2;
    private int powerUpPoolMaxSize = 3;

    void Awake()
    {
        isShowingEndPanel = false;
        currentMoveSpeed = -10f;
        bumpPool = new ObjectPool<GameObject>(CreateBump, OnGetFromBumpPool, OnReleaseToBumpPool, OnDestroyPooledBump, bumpCollectionCheck, bumpPoolCapacity, bumpPoolMaxSize);
        powerUpPool = new ObjectPool<GameObject>(CreatePowerUp, OnGetFromPowerUpPool, OnReleaseToPowerUpPool, OnDestroyPooledPowerUp, powerUpCollectionCheck, powerUpPoolCapacity, powerUpPoolMaxSize);
    }

    void Start()
    {
        GameObject pl = new GameObject();

        // If playerSpawn is set in inspector spawn there else spawn at ghostSpawn 
        if (playerSpawn != null)
        {
            Vector3 spawnPoint = new Vector3(playerSpawn.position.x, playerSpawn.position.y, playerSpawn.position.z);
            pl = Instantiate(player, spawnPoint, Quaternion.identity);
        } else
        {
            Vector3 spawnPoint = new Vector3(ghostSpawn.position.x, ghostSpawn.position.y, ghostSpawn.position.z);
            pl = Instantiate(player, spawnPoint, Quaternion.identity);
        }

        playerController = pl.GetComponent<PlayerController>();

        // Start routines to spawn Bumps & PowerUps
        StartCoroutine(SpawnBumpRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(StartDistanceCounterRoutine());
    }

    void Update()
    {
        if (!isShowingEndPanel && !playerController.IsPlaying)
        {
            canvasController?.canvasAnimator.Play("GameOver");
            DestroyAll();
            isShowingEndPanel = true;
        }
    }

    private IEnumerator StartDistanceCounterRoutine()
    {
        distanceTravelled = 0f;
        orbsCollected = 0;

        while (!isShowingEndPanel)
        {
            // Wait 1 sec
            yield return new WaitForSeconds(1f);

            // Calculate distance
            distanceTravelled += currentMoveSpeed;

            // Set UI value
            canvasController?.SetScore(distanceTravelled);

            //Scale speed every 5s
            if (distanceTravelled % 50 == 0)
            {
                SpeedUpGame();
            }
        }
    }

    private IEnumerator SpawnBumpRoutine()
    {
        while (!isShowingEndPanel)
        {
            // Spawn bump
            GameObject bump = bumpPool.Get();

            //Set Bump Velocity
            bump.GetComponent<BumpController>().SetVelocity(currentMoveSpeed);

            //Add bump to List
            bumpObjects.Add(bump);

            // Set next timer
            nextBumpSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);

            // Waiit for next spwan
            yield return new WaitForSeconds(nextBumpSpawnTime);
        }
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (!isShowingEndPanel)
        {
            // Spawn bump
            GameObject power = powerUpPool.Get();

            //Set Bump Velocity
            power.GetComponent<PowerUpController>().SetVelocity(currentMoveSpeed);

            //Add bump to List
            powerUpObjects.Add(power);

            // Waiit for next spwan
            yield return new WaitForSeconds(10f);
        }
    }

    private void SpeedUpGame()
    {
        currentMoveSpeed -= 5f;
    }

    private GameObject CreateBump()
    {
        GameObject bump = Instantiate(bumpPrefab);
        BumpController bumpController = bump.GetComponent<BumpController>();

        // Move the bump to spawn point
        if (bump != null && bumpSpawn != null)
        {
            bump.transform.position = bumpSpawn.position;
            bump.transform.rotation = bumpSpawn.rotation;
        }

        bumpController.BumpPool = (ObjectPool<GameObject>)bumpPool;
        return bump;
    }

    private void OnGetFromBumpPool(GameObject bump)
    {
        // Move the bump to spawn point
        if (bump != null && bumpSpawn != null)
        {
            bump.transform.position = bumpSpawn.position;
            bump.transform.rotation = bumpSpawn.rotation;
        }

        bump.SetActive(true);
    }

    private void OnReleaseToBumpPool(GameObject bump)
    {
        bump.SetActive(false);
    }

    private void OnDestroyPooledBump(GameObject bump)
    {
        Destroy(bump.gameObject);
    }

    private GameObject CreatePowerUp()
    {
        GameObject power = Instantiate(powerUpPrefab);
        PowerUpController powerController = power.GetComponent<PowerUpController>();

        // Move the bump to spawn point
        if (power != null && powerUpSpawn != null)
        {
            float yOffset = Random.Range(-2f, 2f);
            power.transform.position = new Vector3(powerUpSpawn.position.x, powerUpSpawn.position.y + yOffset, powerUpSpawn.position.z);
            //power.transform.position = powerUpSpawn.position;
            power.transform.rotation = powerUpSpawn.rotation;
        }

        powerController.GameManager = this;
        powerController.PowerUpPool = (ObjectPool<GameObject>)powerUpPool;
        return power;
    }

    private void OnGetFromPowerUpPool(GameObject power)
    {
        // Move the bump to spawn point
        if (power != null && powerUpSpawn != null)
        {
            float yOffset = Random.Range(-2f, 2f);
            power.transform.position = new Vector3(powerUpSpawn.position.x, powerUpSpawn.position.y + yOffset, powerUpSpawn.position.z);

            power.transform.rotation = powerUpSpawn.rotation;
        }

        power.SetActive(true);
    }

    private void OnReleaseToPowerUpPool(GameObject power)
    {
        power.SetActive(false);
    }

    private void OnDestroyPooledPowerUp(GameObject power)
    {
        Destroy(power.gameObject);
    }

    private void DestroyAll()
    {
        // Destroy Bumps
        foreach (var bump in bumpObjects)
        {
            Destroy(bump);
        }

        //Destroy PowerUps
        foreach (var power in powerUpObjects)
        {
            Destroy(power);
        }
    }

    public void AddOrbPoint()
    {
        orbsCollected++;
        canvasController?.SetCollectibles(orbsCollected);
    }

}
