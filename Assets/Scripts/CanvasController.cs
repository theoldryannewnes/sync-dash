using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    public Animator canvasAnimator;
    public TMP_Text scoreText;
    public TMP_Text collectibleText;

    private void Awake()
    {
        scoreText.text = "0";
        collectibleText.text = "0";
    }

    public void SetScore(float score)
    {
        scoreText.text = Mathf.RoundToInt(Mathf.Abs(score)).ToString();
    }

    public void SetCollectibles(int orbs)
    {
        collectibleText.text = orbs.ToString();
    }

}
