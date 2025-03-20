using System.Collections;
using UnityEngine;

public class LightsFlicker : MonoBehaviour
{
    public Light lightOB;
    public AudioSource lightSound;

    public float minFlickerTime = 0.1f; // Minimum flicker interval
    public float maxFlickerTime = 0.5f; // Maximum flicker interval
    public float minOffTime = 20f; // Minimum cooldown time after flickering
    public float maxOffTime = 60f; // Maximum cooldown time after flickering

    private int flickerCount;
    private int flickerTarget;
    private float timer;
    private bool isFlickering = false;
    private bool hasStarted = false;

    private static int activeLights = 0; // Track number of active lights
    private static int maxActiveLights = 2; // Only allow 1-2 lights on at a time

    void Start()
    {
        // Random first-time delay (5-60 sec)
        lightOB.enabled = false; // Ensure light starts OFF
        StartCoroutine(InitialDelay(Random.Range(5f, 60f)));
    }

    IEnumerator InitialDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasStarted = true;
        StartCoroutine(FlickerSequence());
    }

    IEnumerator FlickerSequence()
    {
        while (true)
        {
            // Wait for a free slot if too many lights are on
            while (activeLights >= maxActiveLights)
                yield return null;

            activeLights++;
            lightOB.enabled = true;

            if (lightSound != null)
                lightSound.Play();

            // Random flicker count (2-5 times)
            flickerTarget = Random.Range(2, 6);

            for (flickerCount = 0; flickerCount < flickerTarget; flickerCount++)
            {
                yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
                lightOB.enabled = !lightOB.enabled;

                if (lightOB.enabled && lightSound != null)
                    lightSound.Play();
            }

            // Turn off light
            lightOB.enabled = false;
            activeLights = Mathf.Max(0, activeLights - 1);

            // Cooldown before restarting (20-60 sec)
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }
}
