using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score;

    public void ScorePellet()
    {
        score += 10;
        scoreText.text = score.ToString();
    }

    public void ScorePowerPellet()
    {
        score += 50;
        scoreText.text = score.ToString();
    }

    public void ScoreGhost()
    {
        score += 100;
        scoreText.text = score.ToString();
    }
}
