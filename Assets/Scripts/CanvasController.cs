using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    [SerializeField] private TMP_Text p1_Score;
    [SerializeField] private TMP_Text p1_Orbs;
    [SerializeField] private TMP_Text p2_Score;
    [SerializeField] private TMP_Text p2_Orbs;

    public Animator canvasAnimator;

    private void Awake()
    {
        p1_Score.text = "0";
        p1_Orbs.text = "0";
        p2_Score.text = "0";
        p2_Orbs.text = "0";
    }

    public void P1SetScore(float score)
    {
        p1_Score.text = Mathf.RoundToInt(Mathf.Abs(score)).ToString();
    }

    public void P1SetOrbs(int orbs)
    {
        p1_Orbs.text = orbs.ToString();
    }

    public void P2SetScore(float score)
    {
        p2_Score.text = Mathf.RoundToInt(Mathf.Abs(score)).ToString();
    }

    public void P2SetOrbs(int orbs)
    {
        p2_Orbs.text = orbs.ToString();
    }

    public void P1GameOver()
    {
        Debug.Log("Player Game Over!");
        canvasAnimator.Play("GameOver");
    }

    public void P2GameOver()
    {
        Debug.Log("Ghost Game Over!");
    }

}
