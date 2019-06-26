using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

public enum GradientType
{
    Horizontal,
    Vertical,
    Radial,
    RadialAspectFitWidth,
    RadialAspectFitHeight,
}

public class BackgroundScriptableRenderPass : ScriptableRenderPass
{


    private const string Tag = "BackgroundScriptableRenderPass";
    private const int MaximumColors = 6;
    private readonly Color[] _colorArray = new Color[MaximumColors];
    private readonly float[] _timeArray = new float[MaximumColors];
    private int _shaderGradientColorsPropertyId;
    private int _shaderGradientTimesPropertyId;
    private int _shaderGradientColorsSizePropertyId;
    private int _shaderScreenRatioWidthDividedByHeightPropertyId;
    private int _shaderScreenRatioHeightDividedByWidthPropertyId;
    private int _shaderGradientOriginPropertyId;
    private int _shaderInvertDirectionPropertyId;
    private Mesh _fullScreenMesh;

    public Material _backgroundGradientMaterial;
    public Gradient gradient;
    public bool InvertDirection;
    public GradientType GradientType;
    public float RadialOriginX = 0.5f;
    public float RadialOriginY = 0.5f;
    public float aspect;

    public bool dirty;

    public BackgroundScriptableRenderPass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

        _shaderGradientColorsPropertyId = Shader.PropertyToID("_GradientColors");
        _shaderGradientTimesPropertyId = Shader.PropertyToID("_GradientTimes");
        _shaderGradientColorsSizePropertyId = Shader.PropertyToID("_GradientColorsSize");
        _shaderScreenRatioWidthDividedByHeightPropertyId = Shader.PropertyToID("_ScreenRatioWidthDividedByHeight");
        _shaderScreenRatioHeightDividedByWidthPropertyId = Shader.PropertyToID("_ScreenRatioHeightDividedByWidth");
        _shaderGradientOriginPropertyId = Shader.PropertyToID("_GradientOrigin");
        _shaderInvertDirectionPropertyId = Shader.PropertyToID("_InvertDirection");

    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {

        var commandBuffer = CommandBufferPool.Get(Tag);
        if (_backgroundGradientMaterial != null && dirty) {
            _fullScreenMesh = CreateQuadMesh();
            UpdateMaterial();

            dirty = false;
        }
        commandBuffer.DrawMesh(_fullScreenMesh, Matrix4x4.identity, _backgroundGradientMaterial,0,0);
        context.ExecuteCommandBuffer(commandBuffer);
        CommandBufferPool.Release(commandBuffer);

    }


    private static Mesh CreateQuadMesh()
    {
        var quadMesh = new Mesh();
        quadMesh.SetVertices(new List<Vector3>
        {
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f)
        });

        quadMesh.triangles = new[]
        {
            0, 1, 2, 2, 3, 0
        };

        quadMesh.uv = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f)
        };

        return quadMesh;
    }

    private static string GetKeywodForGradientType(GradientType gradientType)
    {
        switch (gradientType)
        {
            case GradientType.Horizontal:
                return "HORIZONTAL_GRADIENT";
            case GradientType.Vertical:
                return "VERTICAL_GRADIENT";
            case GradientType.Radial:
                return "RADIAL_FIT_GRADIENT";
            case GradientType.RadialAspectFitWidth:
                return "RADIAL_ASPECT_WIDTH_GRADIENT";
            case GradientType.RadialAspectFitHeight:
                return "RADIAL_ASPECT_HEIGHT_GRADIENT";
            default:
                throw new ArgumentOutOfRangeException("gradientType", gradientType, null);
        }
    }

    private void UpdateMaterial()
    {
        var colorKeys = gradient.colorKeys;

        if (colorKeys.Length > MaximumColors)
        {
            Debug.LogWarning("Gradient has more than 4 colors. Only the first 4 will be used...");
        }

        var effectiveColorArraySize = Mathf.Min(MaximumColors, colorKeys.Length);
        int i;
        for (i = 0; i < effectiveColorArraySize; i++)
        {
            _colorArray[i] = colorKeys[i].color;
            _timeArray[i] = colorKeys[i].time;

        }

        for (; i < MaximumColors; i++)
        {
            _colorArray[i] = _colorArray[i - 1];
            _timeArray[i] = 1.0f;

        }

        if (_backgroundGradientMaterial != null) {
            _backgroundGradientMaterial.SetColorArray(_shaderGradientColorsPropertyId, _colorArray);
            _backgroundGradientMaterial.SetFloatArray(_shaderGradientTimesPropertyId, _timeArray);
            _backgroundGradientMaterial.SetFloat(_shaderGradientColorsSizePropertyId, effectiveColorArraySize);
            _backgroundGradientMaterial.SetFloat(_shaderScreenRatioWidthDividedByHeightPropertyId, aspect);
            _backgroundGradientMaterial.SetFloat(_shaderScreenRatioHeightDividedByWidthPropertyId, 1.0f / aspect);
            _backgroundGradientMaterial.SetVector(_shaderGradientOriginPropertyId, new Vector2(RadialOriginX,RadialOriginY));
            _backgroundGradientMaterial.SetFloat(_shaderInvertDirectionPropertyId,InvertDirection?1.0f:0.0f);
            
            var shaderKeywords = _backgroundGradientMaterial.shaderKeywords;
            foreach (var t in shaderKeywords)
            {
                _backgroundGradientMaterial.DisableKeyword(t);
            }
            _backgroundGradientMaterial.EnableKeyword(GetKeywodForGradientType(GradientType));
        }
    }
}
