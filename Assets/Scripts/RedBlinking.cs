using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedBlinking : MonoBehaviour
{public float blinkSpeed = 1f; // How fast it blinks (higher = faster)
    private Image image;
    private Coroutine blinkCoroutine;
    private bool blinking = false;

    public float maxAlpha = 10f; // Max alpha for the effect (in 0-255 range)

    private void Start()
    {
        // Convert Unity's default alpha range from 0-1 to 0-255 for the Image component
        image = GetComponent<Image>();
        if (image == null)
            Debug.LogError("RedBlinking: No Image component found!");

        // Set the initial alpha to 0 (fully transparent)
        SetAlpha(0f);
    }

    public void StartBlinking()
    {
        if (!blinking)
        {
            gameObject.SetActive(true); // Show the image when blinking starts
            blinking = true;
            blinkCoroutine = StartCoroutine(Blink());
        }
    }

    public void StopBlinking()
    {
        if (blinking)
        {
            blinking = false;
            StopCoroutine(blinkCoroutine);

            // Reset the image alpha to 0 (fully transparent)
            SetAlpha(0f);

            gameObject.SetActive(false); // Hide the image after blinking stops
        }
    }

    private IEnumerator Blink()
    {
        float alpha = 0f;
        bool increasing = true;

        while (true)
        {
            // Increase or decrease the alpha each frame
            alpha += (increasing ? 1 : -1) * blinkSpeed * Time.deltaTime;

            if (alpha >= maxAlpha) 
            {
                alpha = maxAlpha ; 
                increasing = false;
            }
            else if (alpha <= 0f)
            {
                alpha = 0f;
                increasing = true;
            }

            // Set the alpha value (0-255)
            SetAlpha(alpha);

            yield return null; // Wait for the next frame
        }
    }

    private void SetAlpha(float alpha)
    {
        if (image != null)
        {
            Color c = image.color;

            // Convert alpha to Unity's default 0-1 range for the image's color
            float unityAlpha = Mathf.Clamp(alpha / 255f, 0f, 1f); // Convert 0-255 alpha to 0-1 range

            // Set the image's color with the new alpha
            c.a = unityAlpha;
            image.color = c;
        }
    }
}
