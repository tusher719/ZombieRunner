using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    public float blinkSpeed = 1f;
    private TextMeshProUGUI textComponent;
    private float alpha = 1f;
    private bool fadingOut = true;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (fadingOut)
        {
            alpha -= blinkSpeed * Time.deltaTime;
            if (alpha <= 0.3f)
            {
                alpha = 0.3f;
                fadingOut = false;
            }
        }
        else
        {
            alpha += blinkSpeed * Time.deltaTime;
            if (alpha >= 1f)
            {
                alpha = 1f;
                fadingOut = true;
            }
        }

        Color color = textComponent.color;
        color.a = alpha;
        textComponent.color = color;
    }
}