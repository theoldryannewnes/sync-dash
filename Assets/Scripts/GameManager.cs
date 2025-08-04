using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{

    [SerializeField] private CanvasController canvasController;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ghost;
    [SerializeField] private GameObject bumpPrefab;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform ghostSpawn;
    [SerializeField] private Transform objectSpawner;

    private List<GameObject> bumpObjects = new List<GameObject>();
    private List<GameObject> powerUpObjects = new List<GameObject>();

    private bool isShowingEndPanel;

    private IObjectPool<GameObject> bumpPool;
    private bool bumpCollectionCheck = true;
    private int bumpPoolCapacity = 10;
    private int bumpPoolMaxSize = 20;

    private IObjectPool<GameObject> powerUpPool;
    private bool powerUpCollectionCheck = true;
    private int powerUpPoolCapacity = 1;
    private int powerUpPoolMaxSize = 1;

    public bool IsPlaying { get { return !isShowingEndPanel; } }

    void Awake()
    {
        isShowingEndPanel = false;
        bumpPool = new ObjectPool<GameObject>(CreateBump, OnGetFromBumpPool, OnReleaseToBumpPool, OnDestroyPooledBump, bumpCollectionCheck, bumpPoolCapacity, bumpPoolMaxSize);
        powerUpPool = new ObjectPool<GameObject>(CreatePowerUp, OnGetFromPowerUpPool, OnReleaseToPowerUpPool, OnDestroyPooledPowerUp, powerUpCollectionCheck, powerUpPoolCapacity, powerUpPoolMaxSize);
    }

    void Start()
    {
        GameObject playerObject;

        // If playerSpawn is set spawn player prefab else spawn ghost prefab
        if (playerSpawn != null)
        {
            Vector3 spawnPoint = new Vector3(playerSpawn.position.x, playerSpawn.position.y, playerSpawn.position.z);
            playerObject = Instantiate(player, spawnPoint, Quaternion.identity);

            playerObject.GetComponent<PlayerController>().Manager = this;
            playerObject.GetComponent<PlayerController>().Canvas = canvasController;
        }
        else
        {
            Vector3 spawnPoint = new Vector3(ghostSpawn.position.x, ghostSpawn.position.y, ghostSpawn.position.z);
            playerObject = Instantiate(ghost, spawnPoint, Quaternion.identity);

            playerObject.GetComponent<GhostController>().Manager = this;
            playerObject.GetComponent<GhostController>().Canvas = canvasController;
        }
    }

    private GameObject CreateBump()
    {
        GameObject bump = Instantiate(bumpPrefab);
        BumpController bumpController = bump.GetComponent<BumpController>();

        bumpController.BumpPool = (ObjectPool<GameObject>)bumpPool;
        return bump;
    }

    private void OnGetFromBumpPool(GameObject bump)
    {
        // Move the bump to spawn point
        if (bump != null && objectSpawner != null)
        {
            bump.transform.position = objectSpawner.position;
            bump.transform.rotation = objectSpawner.rotation;
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

        powerController.PowerUpPool = (ObjectPool<GameObject>)powerUpPool;
        return power;
    }

    private void OnGetFromPowerUpPool(GameObject power)
    {
        // Move the bump to spawn point
        if (power != null && objectSpawner != null)
        {
            power.transform.position = objectSpawner.position;
            power.transform.rotation = objectSpawner.rotation;
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
        //Destroy PowerUps
        foreach (var power in powerUpObjects)
        {
            Destroy(power);
        }

        // Destroy Bumps
        foreach (var bump in bumpObjects)
        {
            if (bump.activeSelf)
            {
                BumpController bc = bump.GetComponent<BumpController>();
                bc.DissolveBump();
            }
        }
    }

    public void SpawnBump(float speed)
    {
        // Spawn
        GameObject bump = bumpPool.Get();

        //Set Velocity
        bump.GetComponent<BumpController>().SetVelocity(speed);

        //Add to List
        bumpObjects.Add(bump);
    }

    public void SpawnPowerUp(float speed, float y)
    {
        // Spawn bump
        GameObject power = powerUpPool.Get();

        PowerUpController powerController = power.GetComponent<PowerUpController>();

        //Set Velocity
        powerController.SetVelocity(speed);

        //Set Y Offset
        powerController.SetYOffset(y);

        //Add to List
        powerUpObjects.Add(power);
    }

    public void BreakLastOrb()
    {
        PowerUpController powerUp = powerUpObjects[0].GetComponent<PowerUpController>();

        powerUp.SetVelocity(0, true);
        powerUp.BreakOrb();
    }

    public void GameFinished()
    {
        DestroyAll();
        isShowingEndPanel = true;
    }

}
