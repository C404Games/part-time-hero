using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HistoryOptionButtonAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text>();
        if (texts[0].text != "")
        {
            rawImages[0].transform.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text>();
        if (texts[0].text != "")
        {
            rawImages[0].transform.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
