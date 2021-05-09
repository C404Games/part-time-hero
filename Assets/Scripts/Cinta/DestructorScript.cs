using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructorScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
