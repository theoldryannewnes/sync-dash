using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GhostController : MonoBehaviour
{
    [Range(0f, 1f)]
    public static float simulatedLag = 0.2f;
    public Slider delaySlider;
    public TMP_Text delayText;

    public float smoothingSpeed = 15f;
    private Vector3 targetPosition;

    private bool _isPlaying;

    private Animator jumpAnimator;
    private CanvasController canvasController;
    private GameManager gameManager;

    private float distanceTravelled;
    private int orbsCollected;

    public CanvasController Canvas { set { canvasController = value; } }
    public GameManager Manager { set { gameManager = value; } }

    void Awake()
    {
        _isPlaying = true;
        targetPosition = transform.position;
        jumpAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        delaySlider = canvasController.GetComponentInChildren<Slider>();
        delayText = delaySlider.transform.GetChild(3).GetComponent<TMP_Text>();
        delaySlider.SetValueWithoutNotify(simulatedLag);
        delaySlider.interactable = true;

        // Add listener to handle text
        delaySlider.onValueChanged.AddListener(UpdateLagValue);

        //Update value at start
        UpdateLagValue(delaySlider.value);

        StartCoroutine(StartDistanceCounterRoutine());
    }

    void Update()
    {
        if (_isPlaying)
        {
            // Ensure smooth movement towards playerPosition
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothingSpeed);
        }

        if (NetworkManager.playerActionQueue.Count > 0)
        {
            PlayerActions nextAction = NetworkManager.playerActionQueue.Peek();

            if (Time.time >= nextAction.Timestamp + simulatedLag)
            {
                PlayerActions currentAction = NetworkManager.playerActionQueue.Dequeue();

                //Update target position
                targetPosition = new Vector3(transform.position.x, currentAction.Position, transform.position.z);

                if (currentAction.Type == PlayerActions.PlayerAction.Jump)
                {
                    DoJump();
                }
                else if (currentAction.Type == PlayerActions.PlayerAction.Bump)
                {
                    _isPlaying = false;
                    canvasController.P2GameOver();
                    gameManager.GameFinished();
                }
                else if (currentAction.Type == PlayerActions.PlayerAction.Orb)
                {
                    orbsCollected++;
                    canvasController.P2SetOrbs(orbsCollected);
                    gameManager.BreakLastOrb();
                }
            }
        }
    }

    private IEnumerator StartDistanceCounterRoutine()
    {
        distanceTravelled = 0f;
        orbsCollected = 0;

        while (_isPlaying)
        {
            // Wait 1 sec
            yield return new WaitForSeconds(1f);

            // Calculate distance
            distanceTravelled += NetworkManager.MoveSpeed;

            // Set UI value
            canvasController.P2SetScore(distanceTravelled);
        }
    }

    public void UpdateLagValue(float value)
    {
        // Update value
        simulatedLag = value;

        //convert to ms and display
        delayText.text = $"{value * 1000:F2} ms";
    }

    private void DoJump()
    {
        jumpAnimator.Play("Jump");
    }

}
