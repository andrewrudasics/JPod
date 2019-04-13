using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WaterSufaceManager : Singleton<WaterSufaceManager> {
    public bool isAboveWater = false;
    public bool isNearSurface = false;
    public Transform player;
    public Transform waterSurface;
    public PostProcessVolume underwaterPostprocessing;
    public PostProcessVolume abovewaterPostprocessing;
    public PostProcessVolume waterSurfacePostprocessing;

    private float threshold = 0.2f;

    public void Update()
    {
        if (player.position.y > waterSurface.position.y) {
             isAboveWater = true;
        } else
        {
            isAboveWater = false;
        }
        if (isNearSurface == false && Mathf.Abs(player.position.y - waterSurface.position.y) < threshold)
        {
            StartCoroutine(StartTransition(1));
        }
    }


    IEnumerator StartTransition(float duration)
    {
        float percentage = 0;
        float initialWeightAbove = abovewaterPostprocessing.weight;
        float initialWeightUnder = underwaterPostprocessing.weight;
        while (percentage < 1)
        {
            percentage += Time.deltaTime;
            waterSurfacePostprocessing.weight = Mathf.Lerp(0, 1, time / duration);
            underwaterPostprocessing.weight = Mathf.Lerp(initialWeightUnder, 0, time / duration);
            abovewaterPostprocessing.weight = Mathf.Lerp(initialWeightAbove, 0, time / duration);
            yield return new WaitForEndOfFrame();
        }

        while (percentage > 0)
        {
            percentage -= Time.deltaTime;
            waterSurfacePostprocessing.weight = Mathf.Lerp(0, 1, time / duration);
            underwaterPostprocessing.weight = Mathf.Lerp(initialWeightUnder, 0, time / duration);
            abovewaterPostprocessing.weight = Mathf.Lerp(initialWeightAbove, 0, time / duration);
            yield return new WaitForEndOfFrame();
        }
        yield return null;

    }

}
