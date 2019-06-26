using UnityEngine;
using UnityEngine.Rendering.LWRP;

[CreateAssetMenu(fileName = "BackgroundRendererFeature",
    menuName = "Pipelie/BackgroundRendererFeature", order = 1)]
public class BackgroundRendererFeature : ScriptableRendererFeature
{
    private BackgroundScriptableRenderPass currentPass;


    [SerializeField] Gradient Gradient = new Gradient();

    Material _backgroundGradientMaterial;
    
    [SerializeField]
    [Range(0f,1f)]
    float _radialOriginX = 0.5f;
    public float RadialOriginX {
        get {
            return _radialOriginX;
        }
        set {
            if (currentPass == null)
                currentPass.RadialOriginX = value;
        }
    }

    [SerializeField]
    [Range(0f,1f)]
    float _radialOriginY = 0.5f;
    public float RadialOriginY {
        get {
            return _radialOriginY;
        }
        set {
            if (currentPass == null)
                currentPass.RadialOriginY = value;
        }
    }

    [SerializeField] bool _invertDirection;
    public bool InvertDirection {
        get {
            return _invertDirection;
        }   
        set {
            if (currentPass == null)
                currentPass.InvertDirection = _invertDirection;
        }
    }

    [SerializeField] GradientType _gradientType;
    public GradientType GradientType {
        get {
            return _gradientType;
        }   
        set {
            if (currentPass == null)
                currentPass.GradientType = _gradientType;
        }
    }

    public float aspect;


    public override void Create()
    {
        if (currentPass == null)
            currentPass = new BackgroundScriptableRenderPass();
            
        var gradientBackgroundShader = Shader.Find("Hidden/GradientBackground");
        _backgroundGradientMaterial = new Material(gradientBackgroundShader);			

        currentPass._backgroundGradientMaterial = _backgroundGradientMaterial;
        currentPass.gradient = Gradient;
        currentPass.InvertDirection = InvertDirection;
        currentPass.GradientType = GradientType;
        currentPass.RadialOriginX = RadialOriginX;
        currentPass.RadialOriginY = RadialOriginY;
        currentPass.aspect = aspect;
        currentPass.dirty = true;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(currentPass);
    }

    public void SetGradientKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys) {
        currentPass.gradient.SetKeys( colorKeys, alphaKeys );
        currentPass.dirty = true;
    }
}

