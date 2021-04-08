using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] frameArray;
    public float framerate = 1f;
    
    private int currentFrame;
    private float timer;
    private Image spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer -= 1f;
            currentFrame = (currentFrame + 1) % frameArray.Length;
            spriteRenderer.sprite = frameArray[currentFrame];
        }
    }
}
