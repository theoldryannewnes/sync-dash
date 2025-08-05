using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NetworkManager : MonoBehaviour
{

    private const float minSpawnTime = 2f;
    private const float maxSpawnTime = 6f;

    [SerializeField] private GameManager p1_Manager;
    [SerializeField] private GameManager p2_Manager;

    private static float currentMoveSpeed;

    private void SpeedUpGame() => currentMoveSpeed -= 2f;

    public static float MoveSpeed {  get { return currentMoveSpeed; } }

    public static Queue<PlayerActions> playerActionQueue = new Queue<PlayerActions>();

    private void Awake()
    {
        currentMoveSpeed = -10f;
    }

    void Start()
    {
        StartCoroutine(SpawnObstaclesRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (p1_Manager.IsPlaying || p2_Manager.IsPlaying)
        {
            // Wait
            yield return new WaitForSeconds(10f);

            //Speed Up
            SpeedUpGame();

            //generate random
            int chance = Random.Range(1, 11);

            switch (chance)
            {
                // 30% chance of spawning powerup
                case > 7:

                    float yoffset = Random.Range(-2, 2);

                    if (p1_Manager.IsPlaying)
                    {
                        p1_Manager.SpawnPowerUp(currentMoveSpeed, yoffset);
                    }

                    if (p2_Manager.IsPlaying)
                    {
                        p2_Manager.SpawnPowerUp(currentMoveSpeed, yoffset);
                    }
                    break;

                // No action
                default:
                    break;
            }
        }
    }

    private IEnumerator SpawnObstaclesRoutine()
    {
        while (p1_Manager.IsPlaying || p2_Manager.IsPlaying)
        {
            if (p1_Manager.IsPlaying)
            {
                p1_Manager.SpawnBump(currentMoveSpeed);
            }

            if (p2_Manager.IsPlaying)
            {
                p2_Manager.SpawnBump(currentMoveSpeed);
            }

            yield return new WaitForSeconds( Random.Range(minSpawnTime, maxSpawnTime) );
        }
    }

}
