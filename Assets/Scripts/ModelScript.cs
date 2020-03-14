using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelScript : MonoBehaviour
{
    public GameObject PacMan;
    public GameObject GoalCompletionSquare;
    public GameObject SequenceBlocksSquare;
    public GameObject BlockAnticipationSquare;
    public GameObject BlockLegitimacySquare;
    public GameObject TotalFrustrationSquare;
    public GameObject GameManager;

    //Final frustration marking
    private float totalFrustration;

    //The 6 Concepts that predict frustration
    private float expectedSatisfaction;
    private float extentOfGoalCompletion;
    private float numberOfSequenceBlocks;
    private float blockLegitimacy;
    private float blockAnticipation;
    private float blockDeliberation;

    //Variables needed to predict the 6 concepts
    //Satisfaction

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGoalCompletion();
        UpdateBlockAnticipation();
        totalFrustration = extentOfGoalCompletion + numberOfSequenceBlocks + blockAnticipation;
        //TotalFrustrationSquare.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f);
    }

    //updates the goal completion concept based on #of (power) pellets collected, time spent playing and time spent standing still
    void UpdateGoalCompletion()
    {
        float pelletInfluence = GameManager.GetComponent<GameManagerScriptOld>().pelletsCollected / 240f;                                            //maxvalue = 1
        float powerPelletInfluence = GameManager.GetComponent<GameManagerScriptOld>().powerPelletsCollected / 4f;                                    //maxvalue = 1;
        float timeInfluence = -(Time.time / 240f);                                                   //maxvalue = inf, 1 on 4 minutes
        float stillInfluence = -(PacMan.GetComponent<PacManMoveScript>().timeSpent * (float)0.01);  //maxvalue = inf, less than timeinf
        extentOfGoalCompletion = pelletInfluence + powerPelletInfluence + timeInfluence + stillInfluence; //maxvalue = 2
        GoalCompletionSquare.GetComponent<Renderer>().material.color = new Color(0f, extentOfGoalCompletion/2f, 0f); //an idea to visualize progress
    }

    //TODO requires death similarity measure
    //This updates whenever pac man dies
    public void UpdateNumberOfSequenceBlocks(int numberOfDeaths, float timeBeforeDeath)
    {
        float deathInfluence = -(numberOfDeaths * 0.33f);
        float timeInfluence = -((240f / timeBeforeDeath) * (1f / 240f));  //has weird curve currently, should prob be linear
        numberOfSequenceBlocks = deathInfluence + timeInfluence; //+deathsimilaritymeasure
        SequenceBlocksSquare.GetComponent<Renderer>().material.color = new Color(Mathf.Abs(numberOfSequenceBlocks), 0f / 2f, 0f); //an idea to visualize progress
    }

    public void UpdateBlockAnticipation()
    {
        //avg distance
        //nearest ghost
        //ghost speed
        //ghost predictability measure
        float curGhostDistanceInfluence = -GameManager.GetComponent<GameManagerScriptOld>().CalcAvgDistance();
        float nearestGhostInfluence = -GameManager.GetComponent<GameManagerScriptOld>().CalcNearestGhost();
        float ghostSpeedInfluence = -GameManager.GetComponent<GameManagerScriptOld>().ghostSpeed;
        //float ghostpredictabilitymeasure

        blockAnticipation = curGhostDistanceInfluence + nearestGhostInfluence + ghostSpeedInfluence;
        BlockAnticipationSquare.GetComponent<Renderer>().material.color = new Color(1 - Mathf.Abs(blockAnticipation) / 30f, 0f, 0f);
    }

    public void UpdateBlockLegitimacy()
    {
        //% time spent moving towards player
        //% time spent on chase behaviour
        float moveTimeTowardPlayerInfluence = 0;
        float chaseBehaviourTimeInfluence = 0;

        blockLegitimacy = moveTimeTowardPlayerInfluence + chaseBehaviourTimeInfluence;
    }

    public void UpdateBlockDeliberation()
    {

    }
}
