using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static float maxBumpSpeed;
    private const float maxSpawnTime = 10f;
    private const float minSpawnTime = 3f;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bumpPrefab;
    [SerializeField] private Transform bumpSpawn;
    [SerializeField] private Animator canvasAnimator;

    private PlayerController playerController;
    private float nextBumpSpawnTime;
    private bool isShowingEndPanel;

    void Start()
    {
        isShowingEndPanel = false;
        maxBumpSpeed = 5f;
        GameObject pl = Instantiate(player, Vector3.up, Quaternion.identity);
        playerController = pl.GetComponent<PlayerController>();
    }

    void Update()
    {
        //print($"isPlaying: {playerController.IsPlaying}");

        if (playerController.IsPlaying && !isShowingEndPanel)
        {
            print("Handling bumps");
            if (nextBumpSpawnTime <= 0)
            {
                // Spawn bump
                GameObject bump = Instantiate(bumpPrefab, bumpSpawn, true);

                // Set next timer
                nextBumpSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            }
        }
        else if (!isShowingEndPanel)
        {
            print("Showing gameover panel");
            canvasAnimator.Play("GameOver");
            isShowingEndPanel = true;
        }

        // Decrement timer
        nextBumpSpawnTime -= Time.deltaTime;
    }

    public static void IncreaseMaxSpeed()
    {
        float change = Mathf.FloorToInt(maxBumpSpeed / 2);
        maxBumpSpeed += change;

        print(maxBumpSpeed);
    }

}
