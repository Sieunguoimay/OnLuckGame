using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public GameObject tobeAddedScoreText;
    public GameObject currentScoreText;
    private float currentScore;
    private float targetScore;
    private bool animationTrigger = false;
    private Vector2 origin;
    // Start is called before the first frame update
    void Start()
    {
        //currentScoreText = GetComponent<Text>();
        Debug.Log("Start " + animationTrigger);
    }


    // Update is called once per frame
    void Update()
    {
        if (animationTrigger)
        {
            float scoreDiff = targetScore - currentScore;
            if (scoreDiff > 1.0f)
            {
                float speed = scoreDiff * 0.2f;
                if (speed < 0.3f) speed = 0.3f;
                currentScore += speed;
            }
            else {
                currentScore = targetScore;
                animationTrigger = false;
            }
            Vector2 shaking = new Vector2(Random.Range(-10,10), Random.Range(-10, 10));
            currentScoreText.transform.localPosition = origin + shaking;
            currentScoreText.GetComponent<Text>().text = ((int)currentScore).ToString();
        }
    }
    public ScoreDisplay SetUp(int tobeAddedScore,int currentScore)
    {
        this.tobeAddedScoreText.GetComponent<Text>().text = "+" + tobeAddedScore;
        this.currentScoreText.GetComponent<Text>().text = currentScore.ToString();

        this.currentScore = (float)currentScore;
        targetScore = (float)(currentScore + tobeAddedScore);
        animationTrigger = true;
        origin = currentScoreText.transform.localPosition;
        Debug.Log("ScoreDisplay SetUp "+ currentScore+" "+ targetScore+" "+ animationTrigger);
        return this;
    }
    public void HideTobeAddedScore()
    {
        tobeAddedScoreText.SetActive(false);
    }
}
