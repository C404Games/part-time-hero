using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingArea : MonoBehaviour
{
    public float respawnTimer;
    public float initialBlockTimer;
    public bool spawnAvailable = true;
    public GameObject powerUp1;
    public GameObject powerUp2;
    public GameObject powerUp3;

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
        respawnTimer += Time.deltaTime;
        if (currentTimer <= 0.0f && spawnAvailable)
        {
            Vector3 pos = RandomPointInBounds(myCollider.bounds);
            GeneratePowerUp(UnityEngine.Random.Range(0, 3), pos);
            respawnTimer = 0;
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
