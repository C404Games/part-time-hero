using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArrowController : MonoBehaviour
{

    public GameObject arrowPrefab;

    private Dictionary<ToggleControl, GameObject> arrows;

    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.OfflineMode)
            return;

        offset = new Vector3(0, 0.15f * Screen.height, 0);
        arrows = new Dictionary<ToggleControl, GameObject>();
        foreach(ToggleControl toggle in FindObjectsOfType<ToggleControl>())
        {
            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrow.transform.position = Camera.main.WorldToScreenPoint(toggle.transform.position) + offset;
            arrows.Add(toggle, arrow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.OfflineMode)
            return;

        foreach (KeyValuePair<ToggleControl,GameObject> entry in arrows)
        {
            if (entry.Key.on)
            {
                entry.Value.SetActive(true);
                entry.Value.transform.position = Camera.main.WorldToScreenPoint(entry.Key.transform.position) + offset;
            }
            else
            {
                entry.Value.SetActive(false);
            }
        }
    }
}
