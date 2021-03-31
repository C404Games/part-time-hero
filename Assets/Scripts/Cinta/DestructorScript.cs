﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructorScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Destroy(other.gameObject);
        }
    }
}
