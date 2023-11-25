using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Crt : VolumeComponent, IPostProcessComponent
{
    public FloatParameter screenBendX = new FloatParameter(1000.0f);
    public FloatParameter screenBendY = new FloatParameter(1000.0f);
    public FloatParameter vignetteAmount = new FloatParameter(0.0f);
    public FloatParameter vignetteSize = new FloatParameter(2.0f);
    public FloatParameter vignetteRounding = new FloatParameter(2.0f);
    public FloatParameter vignetteSmoothing = new FloatParameter(1.0f);

    public FloatParameter scanlinesDensity = new FloatParameter(200.0f);
    public FloatParameter scanlinesSpeed = new FloatParameter(-10.0f);
    public FloatParameter noiseAmount = new FloatParameter(250.0f);
    public Vector2Parameter chromaticRed = new Vector2Parameter(new Vector2());
    public Vector2Parameter chromaticGreen = new Vector2Parameter(new Vector2());
    public Vector2Parameter chromaticBlue = new Vector2Parameter(new Vector2());

    //INTERFACE REQUIREMENT 
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}