using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGradient : MonoBehaviour
{
    [SerializeField]
    BackgroundRendererFeature BackgroundRendererFeature;

    Color StartColor = Color.red, EndColor = Color.blue;

    GradientAlphaKey[] alphaKeys;

    void Start()
    {
        var colorKeys = new []
        {
            new GradientColorKey( StartColor, 0 ),
            new GradientColorKey( EndColor, 1 ),
        };

        alphaKeys = new []
        {
            new GradientAlphaKey( 1, 0 ),
            new GradientAlphaKey( 0, 1 ),
        };

        BackgroundRendererFeature.SetGradientKeys( colorKeys, alphaKeys );
    }

    void Update()
    {
        StartColor.g += (0.5f * Time.deltaTime);
        if (StartColor.g > 1)
            StartColor.g = 0;
        EndColor.b += (0.5f * Time.deltaTime);
        if (EndColor.b > 1)
            EndColor.b = 0;

        var colorKeys = new []
        {
            new GradientColorKey( StartColor, 0 ),
            new GradientColorKey( EndColor, 1 ),
        };
        BackgroundRendererFeature.SetGradientKeys( colorKeys, alphaKeys );
    }
}
