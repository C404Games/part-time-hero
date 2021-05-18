using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerupType
{
    FREEZE, 
    SLOW_TIME,
    FAST_TIME,
    MONSTER,
    FAST_WALK,
    STAR,
    MONEY
}

public class PowerUpBehaviour : MonoBehaviour
{

	public float rotationSpeed = 99.0f;
	public bool reverse = false;
	public PowerupType type;

	void Update()
	{
		if (this.reverse)
			transform.Rotate(new Vector3(0f, 1f, 0f) * Time.deltaTime * this.rotationSpeed);
		else
			transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * this.rotationSpeed);
	}

}