using UnityEngine;
using TMPro;

public class RacerRowUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    
    public void SetData(int position, string racerName, int score)
    {
        positionText.text = position.ToString() + ".";
        nameText.text = racerName;
        scoreText.text = score.ToString();
    }
}