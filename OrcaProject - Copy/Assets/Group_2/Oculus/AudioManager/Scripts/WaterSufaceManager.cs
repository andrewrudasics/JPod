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

    private float threshold = 0.5f;

    public void Update()
    {
        if (player.position.y > waterSurface.position.y) {
             isAboveWater = true;
        } else
        {
            isAboveWater = false;
        }
        SetPostProcessingWeight();
    }


    private void SetPostProcessingWeight()
    {
        float percentage = 0;
        float initialWeightAbove = abovewaterPostprocessing.weight;
        float initialWeightUnder = underwaterPostprocessing.weight;

        percentage = 1 - Mathf.Abs(player.position.y - waterSurface.position.y) / threshold;
        print(percentage);
        waterSurfacePostprocessing.weight = Mathf.Lerp(0, 1, percentage);
        underwaterPostprocessing.weight = Mathf.Lerp(initialWeightUnder, 0, percentage);
        abovewaterPostprocessing.weight = Mathf.Lerp(initialWeightAbove, 0, percentage);
    }

}
