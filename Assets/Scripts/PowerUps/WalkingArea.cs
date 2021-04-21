using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingArea : MonoBehaviour
{
    private float currentTime;
    private float currentGenerationTime;
    public float ratio;
    public float initialBlockTimer;
    public float frequency;
    public bool spawnAvailable = true;
    private int firstRandomNumber;
    private int secondRandomNumber;
    public GameObject powerUp1;
    public GameObject powerUp2;
    public GameObject powerUp3;
    public int teamArea;
    private float punctuationTeam1;
    private float punctuationTeam2;
    private float handicapFactor;

    private BoxCollider myCollider;
    private float currentTimer;

    void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        frequency = 2500;
        ratio = 1.0f;
        secondRandomNumber = -1;
        currentTimer = 0;
        handicapFactor = 1.0f;
        initialBlockTimer = 30;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (initialBlockTimer < currentTime)
        {
            currentGenerationTime += Time.deltaTime;
            secondRandomNumber = (int)UnityEngine.Random.Range((int)currentGenerationTime, frequency);
            if (teamArea == 1 && punctuationTeam1 < punctuationTeam2 || teamArea == 2 && punctuationTeam2 < punctuationTeam1)
            {
                handicapFactor = 1.05f;
            } else
            {
                handicapFactor = 1.0f;
            }
            if (frequency <= (secondRandomNumber + 2)*ratio* handicapFactor)
            {
                Random.seed = 15 + (int)currentTime;
                Vector3 pos = RandomPointInBounds(myCollider.bounds);
                GeneratePowerUp(UnityEngine.Random.Range(0, 3), pos);
                currentGenerationTime = 0;
            }
        }        
    }

    private void GeneratePowerUp(float powerUp, Vector3 pos)
    {
        switch (powerUp)
        {
            case 0:
                {
                    Instantiate(powerUp1, pos, Quaternion.identity);
                    break;
                }
            case 1:
                {
                    Instantiate(powerUp2, pos, Quaternion.identity);
                    break;
                }
            case 2:
                {
                    Instantiate(powerUp3, pos, Quaternion.identity);
                    break;
                }
        }
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

}
