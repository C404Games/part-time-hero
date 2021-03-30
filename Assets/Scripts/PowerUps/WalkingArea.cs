using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingArea : MonoBehaviour
{
    public float respawnTimer = 15;
    public bool spawnAvailable = true;
    public GameObject powerUp1;
    public GameObject powerUp2;

    private BoxCollider myCollider;
    private float currentTimer;

    void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        currentTimer = respawnTimer;
    }

    void Update()
    {
        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0.0f && spawnAvailable)
        {
            Vector3 pos = RandomPointInBounds(myCollider.bounds);
            GeneratePowerUp(UnityEngine.Random.Range(0, 2), pos);
            currentTimer = respawnTimer;
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
