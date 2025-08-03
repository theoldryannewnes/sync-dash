using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private GameManager p1_Manager;
    [SerializeField] private GameManager p2_Manager;

    private static float currentMoveSpeed = -10f;

    public static float MoveSpeed {  get { return currentMoveSpeed; } }

    public static Queue<PlayerActions> playerActionQueue = new Queue<PlayerActions>();

    public static void SpeedUpGame() => currentMoveSpeed -= 5f;

}
