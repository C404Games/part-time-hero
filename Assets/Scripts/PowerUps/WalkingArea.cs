using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public List<string> powerUps;

    public int teamArea;
    private float handicapFactor;

    private BoxCollider myCollider;
    private float currentTimer;

    private MatchManager matchManager;

    void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        //Arreglo temporal, deberían de rellenarse automaticamente
        //desde el inpsector. Pero en vez de eso, se vacian por alguna razon.
        powerUps[0] = "Freeze";
        powerUps[1] = "Money";
        powerUps[2] = "Monster";
        powerUps[3] = "Speed";

        frequency = 5500;
        ratio = 1.0f;
        secondRandomNumber = -1;
        currentTimer = 0;
        handicapFactor = 1.0f;
        initialBlockTimer = 30;
        matchManager = FindObjectOfType<MatchManager>();
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (initialBlockTimer < currentTime)
        {
            currentGenerationTime += Time.deltaTime;
            secondRandomNumber = (int)Random.Range((int)currentGenerationTime, frequency);
            if (teamArea == 1 && matchManager.getPunctuationTeam1() < matchManager.getPunctuationTeam2() || teamArea == 2 && matchManager.getPunctuationTeam2() < matchManager.getPunctuationTeam1())
            {
                handicapFactor = 1.05f;
            } else
            {
                handicapFactor = 1.0f;
            }
            if (frequency <= (secondRandomNumber + 2)*ratio* handicapFactor)
            {
                Random.seed = 15 + (int)currentTime;
                Vector3 pos = RandomPointInBounds();
                GeneratePowerUp();
                currentGenerationTime = 0;
                currentTime = 0;
            }
        }        
    }

    private void GeneratePowerUp()
    {
        if (PhotonNetwork.OfflineMode || PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PowerUps", powerUps[(int)Random.Range(0, powerUps.Count)]), RandomPointInBounds(), Quaternion.identity);
        }
    }

    public Vector3 RandomPointInBounds()
    {
        Bounds bounds = myCollider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

}
