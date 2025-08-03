using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{

    private const float maxSpawnTime = 8f;
    private const float minSpawnTime = 3f;

    [SerializeField] private CanvasController canvasController;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ghost;
    [SerializeField] private GameObject bumpPrefab;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform ghostSpawn;
    [SerializeField] private Transform bumpSpawn;
    [SerializeField] private Transform powerUpSpawn;

    private List<GameObject> bumpObjects = new List<GameObject>();
    private List<GameObject> powerUpObjects = new List<GameObject>();

    private float nextBumpSpawnTime;
    private bool isShowingEndPanel;

    private IObjectPool<GameObject> bumpPool;
    private bool bumpCollectionCheck = true;
    private int bumpPoolCapacity = 10;
    private int bumpPoolMaxSize = 20;

    private IObjectPool<GameObject> powerUpPool;
    private bool powerUpCollectionCheck = true;
    private int powerUpPoolCapacity = 1;
    private int powerUpPoolMaxSize = 1;

    public float currentMoveSpeed;

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

            pl.GetComponent<PlayerController>().Manager = this;
            pl.GetComponent<PlayerController>().Canvas = canvasController;
        } else
        {
            Vector3 spawnPoint = new Vector3(ghostSpawn.position.x, ghostSpawn.position.y, ghostSpawn.position.z);
            pl = Instantiate(ghost, spawnPoint, Quaternion.identity);

            pl.GetComponent<GhostController>().Manager = this;
            pl.GetComponent<GhostController>().Canvas = canvasController;
        }

        // Start routines to spawn Bumps & PowerUps
        StartCoroutine(SpawnBumpRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    void Update()
    {

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
            power.transform.rotation = powerUpSpawn.rotation;
        }

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

    public void GameFinished()
    {
        DestroyAll();
        isShowingEndPanel = true;
    }

}
