using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{

    public GameObject[] highlights;

    public string text;

    public GameObject arrowPrefab;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    private void OnEnable()
    {

        offset = new Vector3(0, 0.1f * Screen.height, 0);

        for(int i = 0; i < highlights.Length; i++)
        {
            GameObject arrowInstance = Instantiate(arrowPrefab, transform);
            arrowInstance.transform.position = Camera.main.WorldToScreenPoint(highlights[i].transform.position) + offset;
        }
    }
}
