using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialClockController : MonoBehaviour
{
    public GameObject radialClockPrefab;
    //List<StationInstance> stations;
    Dictionary<StationInstance, Image> clocks;

    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, 0.15f * Screen.height, 0);
        clocks = new Dictionary<StationInstance, Image>();
        List<StationInstance> stations = new List<StationInstance>(FindObjectsOfType<StationInstance>());
        foreach(StationInstance station in stations)
        {
            GameObject clock = Instantiate(radialClockPrefab);
            clock.transform.SetParent(transform);
            Image clockImage = clock.GetComponent<Image>();
            clockImage.fillAmount = 0;
            clockImage.enabled = false;
            clock.transform.position = Camera.main.WorldToScreenPoint(station.transform.position) + offset;
            clocks.Add(station, clockImage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyValuePair<StationInstance, Image> entry in clocks)
        {
            StationInstance station = entry.Key;
            Image clock = entry.Value;
            if (station.isBusy() && station.transitionTime > 0)
            {
                clock.enabled = true;
                float currTime = station.currentTransitionTime;
                float time = station.transitionTime;
                clock.fillAmount = Mathf.Lerp(0, 1, currTime / time);
            }
            else
            {
                clock.enabled = false;
            }
        }
    }
}
